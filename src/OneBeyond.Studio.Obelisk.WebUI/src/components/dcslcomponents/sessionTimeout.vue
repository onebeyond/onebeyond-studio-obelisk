<template>
    <div>
        <v-modalPopup
            :visible="showLogoutDialog"
            :title="$t('message.timeoutTitle')"
            :message="$t('message.timeoutMessage')"
            @close="cancelLogout"
        >
            <template slot="footer">
                <button
                    type="button"
                    id="cancelLogoutBtn"
                    class="btn btn-secondary"
                    v-on:click="cancelLogout"
                >{{ $t('button.keepWorking') }}</button>
                <button
                    id="logoutBtn"
                    class="btn btn-danger"
                    v-on:click="doLogout"
                >{{ $t('button.signOut') }} ({{ secondsLeft }})</button>
            </template>
        </v-modalPopup>
    </div>
</template>

<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import differenceInSeconds from "date-fns/differenceInSeconds";
    import LocalSessionStorage from "@js/stores/localSessionStorage";
    import sessionTimeoutDictionary from "@js/localizations/resources/components/sessionTimeout";
    import AccountApiClient from "@js/api/accounts/accountApiClient";
    import Configuration from "@js/configuration/configuration";

    @Component({
        name: "sessionTimeout",
        components: {},
        i18n: {
            messages: sessionTimeoutDictionary
        }
    })
    export default class SessionTimeout extends Vue {
        secondsBeforeDisplayingDialog: number;
        jsIntervalReference: any;
        showLogoutDialog: boolean;
        secondsLeft: number;
        accountApiClient: AccountApiClient;
        sessionDurationInMinutes: number = 60;

        constructor() {
            super();
            this.secondsBeforeDisplayingDialog = 10;
            this.jsIntervalReference = null;
            this.showLogoutDialog = false;
            this.secondsLeft = 0;
            this.accountApiClient = new AccountApiClient();
        }

        created(): void {
            // We can retrieve the setting from the configuration as we can be sure it has been already loaded
            this.sessionDurationInMinutes = Configuration.appSettings.sessionTimeoutInMinutes;

            let self = this;
            this.jsIntervalReference = setInterval(function () {
                let expiryDate: Date = new Date(LocalSessionStorage.getLastServerRequestDate() + (self.sessionDurationInMinutes * 60 * 1000));
                self.secondsLeft = Math.round((differenceInSeconds(expiryDate, new Date())));
                self.showLogoutDialog = self.secondsLeft < self.secondsBeforeDisplayingDialog;
                if (self.secondsLeft < 0) {
                    self.doLogout();
                }
            }, 2000);
        }

        public cancelLogout(): void {
            //Calling ping action will in turn reset the clock and hence cancelling the dialog
            this.accountApiClient.ping();
        }

        public async doLogout(): Promise<void> {
            clearInterval(this.jsIntervalReference); //Stopping timer since reload will happen anyway
            this.showLogoutDialog = false;

            await this.accountApiClient.signOut();

            window.location.href = `${(window as any).location.origin}/auth/`;
        }
    }

</script>
