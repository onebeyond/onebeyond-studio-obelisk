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
                <form :action="logoutUrl" method="post" id="sessionTimeoutLogoutForm">
                    <button
                        type="submit"
                        id="logoutBtn"
                        class="btn btn-danger"
                        v-on:click="doLogout"
                    >{{ $t('button.logout') }} ({{ secondsLeft }})</button>
                </form>
            </template>
        </v-modalPopup>
    </div>
</template>

<script lang="ts">
    import { Vue, Component, Prop } from "vue-property-decorator";
    import differenceInSeconds from "date-fns/differenceInSeconds";
    import LocalSessionStorage from "@js/stores/localSessionStorage";
    import sessionTimeoutDictionary from "@js/localizations/resources/components/sessionTimeout";
    import AccountApiClient from "@js/api/accounts/accountApiClient";

    @Component({
        name: "sessionTimeout",
        components: {},
        i18n: {
            messages: sessionTimeoutDictionary
        }
    })
    export default class SessionTimeout extends Vue {
        @Prop({ type: Number, required: true }) sessionDurationInMinutes!: any;

        secondsBeforeDisplayingDialog: number;
        jsIntervalReference: any;
        showLogoutDialog: boolean;
        secondsLeft: number;
        accountApiClient: AccountApiClient;

        constructor() {
            super();
            this.secondsBeforeDisplayingDialog = 10;
            this.jsIntervalReference = null;
            this.showLogoutDialog = false;
            this.secondsLeft = 0;
            this.accountApiClient = new AccountApiClient();
        }

        created(): void {
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

        public get logoutUrl(): string {
            return `${this.$rootPath}Account/Logout`;
        }

        public cancelLogout(): void {
            //Calling ping action will in turn reset the clock and hence cancelling the dialog
            this.accountApiClient.ping();
        }

        public doLogout(): void {
            clearInterval(this.jsIntervalReference); //Stopping timer since reload will happen anyway
            this.showLogoutDialog = false;
            const logoutForm = document.getElementById("sessionTimeoutLogoutForm") as HTMLFormElement;
            logoutForm.submit();
        }
    }

</script>
