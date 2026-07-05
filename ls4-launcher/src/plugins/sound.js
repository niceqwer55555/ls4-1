import { uuid } from "uuidv4";
import generalTemplates from "./soundTemplates/general";

const Sound = {
  install(Vue, { store }) {
    this.EventBus = new Vue();

    Vue.prototype.$sound = {
      playingMusics: [],
      musicEnabled: true,
      toggleMusic(value) {
        if (this.musicEnabled) {
          this.stopAll("ALL");
        }

        if (value) {
          this.musicEnabled = value;
        } else {
          this.musicEnabled = !this.musicEnabled;
        }

        return this.musicEnabled;
      },
      getPath(params) {
        let path;

        if (params.local) {
          path = "static/audio/";
        } else {
          const { host, port } = store.state.config.download;
          path = `${host}:${port}/audio/`;
        }

        switch (params.state) {
          case "CHAMPSELECT_GENERAL":
            path += "champselect/";
            break;
          case "CHAMPSELECT_CLICK":
            path += "champselect/click/";
            break;
          case "CHAMPSELECT_CHAMPIONS":
            path += "champselect/champions/";
            break;
          case "CHAMPSELECT_MUSIC":
            path += "champselect/music/";
            break;
          case "CHAMPSELECT_PHASE":
            path += "champselect/phase/";
            break;
          case "LOGIN":
            path += "login/";
            break;
          case "OVERVIEW":
            path += "overview/";
            break;
          case "CORE_MUSIC":
            path += "core/music/";
            break;
          case "CORE":
            path += "core/";
            break;
          default:
            path += "home/";
            break;
        }

        path += params.fileName;

        return path;
      },
      play(params) {
        const path = this.getPath(params);

        const audio = new Audio(path);
        if (!params.volume) {
          audio.volume = 0.25;
        } else {
          audio.volume = params.volume;
        }

        if (params.type === "MUSIC") {
          if (this.musicEnabled) {
            let audioId = params.state + "_" + uuid();

            if (params.loop && params.loop === true) {
              audio.loop = true;
              audioId += "_LOOP";
            }

            audio.onended = function() {
              if (typeof this.playingMusics[audioId] !== undefined) {
                delete this.playingMusics[audioId];
              }
            };

            this.playingMusics[audioId] = audio;
            audio.play();
            return audioId;
          }
        } else {
          audio.play();
        }
      },
      asyncPlay(params) {
        const path = this.getPath(params);

        return new Promise((resolve, reject) => {
          try {
            const audio = new Audio(path);
            audio.onended = function() {
              resolve(true);
            };
            audio.play();
          } catch (error) {
            reject(error);
          }
        });
      },
      isPlaying(audioId) {
        const audio = this.playingMusics[audioId];
        return (
          audio.currentTime > 0 &&
          !audio.paused &&
          !audio.ended &&
          audio.readyState > audio.HAVE_CURRENT_DATA
        );
      },
      stop(audioId) {
        if (this.isPlaying(audioId)) {
          this.playingMusics[audioId].pause();
          this.playingMusics[audioId].currentTime = 0;

          delete this.playingMusics[audioId];
        }
      },
      stopAll(state) {
        for (let key in this.playingMusics) {
          if (key.includes(state) || state === "ALL") {
            this.playingMusics[key].pause();
            this.playingMusics[key].currentTime = 0;

            delete this.playingMusics[key];
          }
        }
      },
      stopAllLoops(state) {
        for (let key in this.playingMusics) {
          if (key.includes("LOOP") && (key.includes(state) || state == "ALL")) {
            if (this.isPlaying(key)) {
              this.playingMusics[key].pause();
              this.playingMusics[key].currentTime = 0;

              delete this.playingMusics[key];
            }
          }
        }
      },
      isMusicPlaying(state) {
        Object.keys(this.playingMusics).forEach(c => {
          if (c.includes(state)) {
            return true;
          }
        });

        return false;
      },
      template(templateId) {
        let templates = { ...generalTemplates };

        if (templates[templateId]) {
          return this.play(templates[templateId]);
        } else {
          throw new Error(`Sound ${templateId} does not exist`);
        }
      }
    };
  }
};

export default Sound;
