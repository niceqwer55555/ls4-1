import Vue from "vue";
import VueRouter from "vue-router";
import Login from "@/views/LoginPage.vue";
import LoggedIn from "@/views/LoggedIn.vue";

import Lobby from "@/views/Lobby.vue";
import LobbyCustom from "@/views/LobbyCustom.vue";

import ChampSelect from "@/views/ChampSelect.vue";

import Play from "@/views/LoggedIn/Play.vue";

import HomeOverview from "@/views/Home/Overview.vue";

import Profile from "@/views/LoggedIn/Profile.vue";
import ProfileOverview from "@/views/Profile/Overview.vue";

import Store from "@/views/Store/Store.vue";
import StoreSkins from "@/views/Store/Skins.vue";
import StoreIcons from "@/views/Store/Icons.vue";
import StoreBoosts from "@/views/Store/Boosts.vue";
import StoreLauncher from "@/views/Store/Launcher.vue";
import StoreAccount from "@/views/Store/Account.vue";

import Collection from "@/views/LoggedIn/Collection.vue";
import CollectionChampions from "@/views/Collection/Champions.vue";

import Error from "@/views/Error.vue";
import Bugreport from "@/views/BugReport.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Login",
    component: Login
  },
  {
    path: "/lobby",
    name: "Lobby",
    component: Lobby
  },
  {
    path: "/lobbyCustom",
    name: "LobbyCustom",
    component: LobbyCustom
  },
  {
    path: "/champselect",
    name: "ChampSelection",
    component: ChampSelect
  },
  {
    path: "/LoggedIn",
    name: "LoggedIn",
    component: LoggedIn,
    redirect: "/LoggedIn/home",
    children: [
      {
        path: "/LoggedIn/play",
        name: "Play",
        component: Play
      },
      {
        path: "/LoggedIn/home",
        name: "Home",
        component: HomeOverview
      },
      {
        path: "/LoggedIn/profile",
        name: "Profile",
        component: Profile,
        redirect: "/LoggedIn/profile/overview",
        children: [
          {
            path: "/LoggedIn/profile/overview",
            name: "ProfileOverview",
            component: ProfileOverview
          }
        ]
      },
      {
        path: "/LoggedIn/store",
        name: "Store",
        component: Store,
        redirect: "/LoggedIn/store/skins",
        children: [
          {
            path: "/LoggedIn/store/skins",
            name: "StoreSkins",
            component: StoreSkins
          },
          {
            path: "/LoggedIn/store/icons",
            name: "StoreIcons",
            component: StoreIcons
          },
          {
            path: "/LoggedIn/store/boosts",
            name: "StoreBoosts",
            component: StoreBoosts
          },
          {
            path: "/LoggedIn/store/launcher",
            name: "StoreLauncher",
            component: StoreLauncher
          },
          {
            path: "/LoggedIn/store/account",
            name: "StoreAccount",
            component: StoreAccount
          }
        ]
      },
      {
        path: "/LoggedIn/collection",
        name: "Collection",
        component: Collection,
        redirect: "/LoggedIn/collection/champions",
        children: [
          {
            path: "/LoggedIn/collection/champions",
            name: "Champions",
            component: CollectionChampions
          }
        ]
      }
    ]
  },
  {
    path: "*",
    name: "Error",
    component: Error
  },
  {
    path: "/bugreport",
    name: "BugReport",
    component: Bugreport
  }
];

const router = new VueRouter({
  routes
});

export default router;
