import { readdirSync, readFileSync } from "fs";
import { join } from "path";
import { ipcRenderer } from "electron";

// Somehow this is undefined
const __STATIC = ipcRenderer.invoke("getStatic").then(result => {
  return result;
});

// We need a vue instance to handle reactivity
var vm = null;

const VueTranslate = {
  install(Vue) {
    const version = Vue.version[0];

    if (!vm) {
      vm = new Vue({
        data() {
          return {
            current: "",
            locales: {},
            loadedlanguages: [],
            loadedOnce: false
          };
        },

        computed: {
          // Current selected language
          lang() {
            return this.current;
          },

          // Current locale values
          locale() {
            if (!this.locales[this.current]) return null;

            return this.locales[this.current];
          }
        },

        methods: {
          // Set a language as current
          setLang(val) {
            if (this.current !== val) {
              if (this.current === "") {
                this.$emit("language:init", val);
              } else {
                this.$emit("language:changed", val);
              }
            }

            this.current = val;

            this.$emit("language:modified", val);
          },

          // Get the loaded languages
          getLanguages() {
            return this.loadedlanguages;
          },

          printMissing() {
            if (process.env.VUE_APP_VERSION !== "undefined") return;
            let languages = this.getLanguages();

            if (languages.length == 0) return;

            const enus_lang = languages.filter(language => {
              return language.code == "en_US";
            })[0].locale;
            let shouldHave = Object.keys(enus_lang);

            let doesntHave = {
              date: new Date(),
              keys_en: {},
              langs: {}
            };

            languages.forEach(language => {
              let has = Object.keys(language.locale);
              shouldHave.forEach(sh_key => {
                if (!has.includes(sh_key)) {
                  if (typeof doesntHave.langs[language.code] == "undefined") {
                    doesntHave.langs[language.code] = [];
                  }
                  doesntHave.langs[language.code].push(sh_key);
                  doesntHave.keys_en[sh_key] = enus_lang[sh_key];
                }
              });
            });

            if (Object.keys(doesntHave).length > 0) {
              ipcRenderer.send("translate_missing_langkeys", doesntHave);
            }
          },

          // Load static lanugages safely
          safeLoadLanguages(path, defaultLang) {
            if (!this.loadedOnce) {
              this.loadLanguages(path, defaultLang);
              return true;
            } else {
              return false;
            }
          },

          // Load static languages
          loadLanguages(path, defaultLang) {
            this.loadedOnce = true;
            let langFolderPath = path;

            if (!langFolderPath) {
              __STATIC.then(result => {
                langFolderPath = join(result, "lang");
                this.loadLanguagesInternal(
                  readdirSync(langFolderPath),
                  langFolderPath,
                  defaultLang
                );
              });
            } else {
              this.loadLanguagesInternal(
                readdirSync(langFolderPath),
                langFolderPath,
                defaultLang
              );
            }
          },

          loadLanguagesInternal(langDir, langFolderPath, defaultLang) {
            let locales = {};

            langDir.forEach(file => {
              const fileContents = readFileSync(
                join(langFolderPath, file),
                "utf-8"
              );
              try {
                const jsonContent = JSON.parse(fileContents);
                locales[jsonContent.code] = jsonContent.locale;
                // delete jsonContent.locale;
                this.loadedlanguages.push(jsonContent);
                console.log(
                  `Language '${jsonContent.language}' successfully loaded. Made by ${jsonContent.author}.`
                );
              } catch (error) {
                console.error(error);
                console.log(`One of the languages failed to load. (${file})`);
              }
            });

            this.setLocales(locales);
            if (defaultLang && typeof locales[defaultLang] !== "undefined") {
              this.setLang(defaultLang);
            }
            this.printMissing();
          },

          // Set a locale to use
          setLocales(locales) {
            if (!locales) return;

            let newLocale = Object.create(this.locales);

            for (let key in locales) {
              if (!newLocale[key]) newLocale[key] = {};

              Vue.util.extend(newLocale[key], locales[key]);
            }

            this.locales = Object.create(newLocale);

            this.$emit("locales:loaded", locales);
          },

          text(t) {
            if (!this.locale || !this.locale[t]) {
              return t;
            }

            return this.locale[t];
          },

          textWithParams(t, params = null) {
            if (!this.locale || !this.locale[t]) {
              return t;
            }

            if (!params || params === null || typeof params === "undefined") {
              return t;
            }

            Object.keys(params).forEach((key, index) => {
              if (index > 0) {
                t = t.replace(`%${key}%`, params[key]);
              } else {
                t = this.locale[t].replace(`%${key}%`, params[key]);
              }
            });

            return t;
          }
        }
      });

      Vue.prototype.$translate = vm;
    }

    // Mixin to read locales and add the translation method and directive
    Vue.mixin({
      [version === "1" ? "init" : "beforeCreate"]() {
        this.$translate.setLocales(this.$options.locales);
      },

      methods: {
        // An alias for the .$translate.text method
        t(t) {
          return this.$translate.text(t);
        },

        tWithParams(t, params) {
          return this.$translate.textWithParams(t, params);
        }
      },

      directives: {
        translate: function(el) {
          if (!el.$translateKey) el.$translateKey = el.innerText;

          let text = this.$translate.text(el.$translateKey);

          el.innerText = text;
        }.bind(vm)
      }
    });

    // Global method to load static locales
    Vue.loadLanguages = (path = null, defaultLang = null) => {
      vm.$translate.loadLanguages(path, defaultLang);
    };

    // Global method for loading locales
    Vue.locales = locales => {
      vm.$translate.setLocales(locales);
    };

    // Global method for setting languages
    Vue.lang = lang => {
      vm.$translate.setLang(lang);
    };
  }
};

export default VueTranslate;
