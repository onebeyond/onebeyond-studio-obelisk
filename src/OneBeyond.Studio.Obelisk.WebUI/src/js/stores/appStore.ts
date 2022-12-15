import Vue from 'vue';
import Vuex from 'vuex';
import { UserContext } from '../dataModels/users/userContext';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        userContext: new UserContext()
    },

    mutations: {
        setUserContext(state, userContext: UserContext) {
            state.userContext = userContext;
        }
    }
});