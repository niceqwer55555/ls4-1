const { defineConfig } = require('@vue/cli-service')

process.env.VUE_APP_VERSION = process.env.CI_COMMIT_SHORT_SHA
process.env.VUE_APP_ENV = process.env.APP_ENV
process.env.VUE_APP_CDN = process.env.APP_CDN
process.env.VUE_APP_CDN_PORT = process.env.APP_CDN_PORT
process.env.VUE_APP_API = process.env.APP_API
process.env.VUE_APP_API_PORT = process.env.APP_API_PORT

module.exports = defineConfig({
  transpileDependencies: true,
  configureWebpack: {
    resolve: {
      fallback: {
        fs: false,
        path: require.resolve('path-browserify'),
        child_process: false,
        stream: false,
        crypto: false,
        os: false,
        net: false,
        tls: false,
        zlib: false,
        http: false,
        https: false,
        assert: false,
        util: false,
        buffer: false,
        string_decoder: false,
        events: false,
        url: false,
        punycode: false,
        querystring: false,
        'fs-extra': false,
        electron: false
      }
    }
  },
  pluginOptions: {
    electronBuilder: {
      nodeIntegration: true,
      builderOptions: {
        appId: "com.leagues4.launcher",
        productName: "LeagueS4 Launcher",
        copyright: "Copyright (C) 2020  LeagueS4",
        directories: {
          output: "D:/game/ls4-1/ls4-launcher/dist-build2"
        },
        publish: [
          {
            provider: "generic",
            url: "https://git.jandev.de"
          }
        ]
      },
      customFileProtocol: "./"
    }
  }
})