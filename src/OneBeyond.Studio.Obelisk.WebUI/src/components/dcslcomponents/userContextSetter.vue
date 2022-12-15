<template>
    <div></div>
</template>

<script lang="ts">
    import UserApiClient from "@js/api/users/userApiClient";
    import { UserContext } from "@js/dataModels/users/userContext";
    import LocalSessionStorage from "@js/stores/localSessionStorage";
    import { Vue, Component } from "vue-property-decorator"

    @Component({
        name: "UserContextSetter",
        components: {}
    })
    export default class UserContextSetter extends Vue {
        private userApiClient: UserApiClient;

        constructor() {
            super();
            this.userApiClient = new UserApiClient();
        }

        async created(): Promise<void> {
            if (this.$store.state.userContext.isEmpty()) {
                try {
                    const userContext: UserContext = await this.userApiClient.whoAmI();
                    this.$store.commit('setUserContext', userContext);
                }
                catch {
                    // Redirect to Login page if unable to get the User Context
                    LocalSessionStorage.setUserAuthenticated(false);
                    window.location.href = `${(window as any).location.origin}/auth/`;
                }
            }
        }
    }
</script>
