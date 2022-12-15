import Vue from 'vue';
import Vuex from 'vuex';
import { UserContext } from '../dataModels/users/userContext';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        userContext: UserContext.Empty()
    },

    mutations: {
        setUserContext(state, userContextJson: string) {
            state.userContext = UserContext.FromJson(userContextJson);
        }
    }
});