import axios from "axios";

export default {
  getStoreItems({ state }, type) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/shop/${type}`, config)
      .then(function(response) {
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  getStoreCheckName({ state }, name) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/users/${name}`, config)
      .then(function(response) {
        console.log(response);
        return false;
      })
      .catch(err => {
        console.log(err);
        return true;
      });
  },
  postPurchaseItem({ state }, { item, category, desiredName }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    let postitem = {
      id: item.id,
      expectedPrice: item.price,
      category: category
    };
    if (typeof desiredName != "undefined") {
      postitem.summonerName = desiredName;
    }

    //TODO: Handle summoner name change.
    return axios
      .post(`${host}:${port}/shop/purchase`, postitem, config)
      .then(function(response) {
        console.log(response);
        return response;
      })
      .catch(err => {
        console.log(err.response.data);
        throw err;
      });
  }
};
