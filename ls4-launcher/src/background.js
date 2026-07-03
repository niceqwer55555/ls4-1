"use strict";

import { app, protocol, BrowserWindow, ipcMain, dialog } from "electron";
// import { createProtocol } from "vue-cli-plugin-electron-builder/lib";
import installExtension, { VUEJS_DEVTOOLS } from "electron-devtools-installer";
import ping from "ping";
import { join } from "path";
import { writeFileSync } from "fs";

const isDevelopment = process.env.NODE_ENV !== "production";

// UPDATER
const updater = require("electron-updater");
const autoUpdater = updater.autoUpdater;
let updateBlocked = false;

autoUpdater.autoDownload = true;

autoUpdater.setFeedURL({
  provider: "generic",
  channel: "latest",
  url:
    "https://git.jandev.de/api/v4/projects/102/jobs/artifacts/master/raw/dist_electron?job=Build Production"
});

autoUpdater.on("checking-for-update", function() {
  updateBlocked = true;
  win.webContents.send("update", "Checking for update...");
});

autoUpdater.on("update-not-available", function() {
  updateBlocked = false;
  win.webContents.send("update", "No Update available.");
});

autoUpdater.on("update-available", function() {
  win.webContents.send("update", "Downloading update...");
});

autoUpdater.on("update-downloaded", function() {
  win.webContents.send("update", "Restarting...");
  setTimeout(function() {
    autoUpdater.quitAndInstall();
  }, 5000);
});

autoUpdater.on("error", function(error) {
  win.webContents.send("update", "Update error: " + error);
});

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let win;

// Scheme must be registered before the app is ready
protocol.registerSchemesAsPrivileged([
  { scheme: "app", privileges: { secure: true, standard: true } }
]);

var enableDevTools = !app.isPackaged;

function createWindow() {
  // Create the browser window.
  win = new BrowserWindow({
    width: 1280,
    height: 720,
    resizable: false,
    autoHideMenuBar: false,
    backgroundColor: "#00ffffff",
    transparent: true,
    fullscreen: false,
    fullscreenable: false,
    frame: false,
    maximizable: false,
    webPreferences: {
      spellcheck: false,
      devTools: enableDevTools,
      // Use pluginOptions.nodeIntegration, leave this alone
      // See nklayman.github.io/vue-cli-plugin-electron-builder/guide/security.html#node-integration for more info
      nodeIntegration: process.env.ELECTRON_NODE_INTEGRATION,
      contextIsolation: false
    }
  });

  if (process.env.WEBPACK_DEV_SERVER_URL) {
    // Load the url of the dev server if in development mode
    win.loadURL(process.env.WEBPACK_DEV_SERVER_URL);
    if (!process.env.IS_TEST) win.webContents.openDevTools();
  } else {
    // createProtocol("app");
    // Load the index.html when not in development
    win.loadURL(`file://${__dirname}/index.html`);

    autoUpdater.checkForUpdatesAndNotify();
  }

  win.on("closed", () => {
    win = null;
  });

  win.on("close", () => {
    win = null;
  });
}

// Quit when all windows are closed.
app.on("window-all-closed", () => {
  // On macOS it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  // On macOS it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (win === null) {
    createWindow();
  }
});

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on("ready", async () => {
  if (isDevelopment && !process.env.IS_TEST) {
    // Install Vue Devtools
    try {
      await installExtension(VUEJS_DEVTOOLS);
    } catch (e) {
      console.error("Vue Devtools failed to install:", e.toString());
    }
  }

  createWindow();
});

// Exit cleanly on request from parent process in development mode.
if (isDevelopment) {
  if (process.platform === "win32") {
    process.on("message", data => {
      if (data === "graceful-exit") {
        app.quit();
      }
    });
  } else {
    process.on("SIGTERM", () => {
      app.quit();
    });
  }
}

ipcMain.on("minimizeApp", () => {
  win.minimize();
});

ipcMain.on("focusApp", () => {
  win.setAlwaysOnTop(true);
  win.setAlwaysOnTop(false);
});

ipcMain.on("closeApp", () => {
  app.exit(0);
});

ipcMain.handle("ping", async (event, domain) => {
  return await ping.promise.probe(domain, { min_reply: 3 });
});

ipcMain.handle("getPath", async () => {
  return app.getPath("userData");
});

ipcMain.handle("openFileDialog", async () => {
  return dialog.showOpenDialog({ properties: ["openFile"] }).then(result => {
    if (result && result.filePaths && result.filePaths.length == 1) {
      return result.filePaths[0];
    }
  });
});

ipcMain.handle("getStatic", async () => {
  return join(__static, "static");
});

ipcMain.handle("checkForUpdate", async () => {
  if (!updateBlocked) {
    autoUpdater.checkForUpdatesAndNotify();
  }
});

ipcMain.on("translate_missing_langkeys", async (event, data) => {
  writeFileSync("missing_langkeys.json", JSON.stringify(data, null, 2));
});
