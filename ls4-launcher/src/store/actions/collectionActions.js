import axios from "axios";

export default {
  getCollectionChampions({ commit, state }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/users/collection/champions`, config)
      .then(function(response) {
        commit("setCollectionChampions", response.data);
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  setSelectedChampion({ commit }, champion) {
    commit("setSelectedChampion", champion);
  },
  getCollectionIcons({ commit, state }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/users/collection/icons`, config)
      .then(function(response) {
        commit("setCollectionIcons", response.data);
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  }
};
