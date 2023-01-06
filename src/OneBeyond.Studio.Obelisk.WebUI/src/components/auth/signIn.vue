<template>
    <div>
        <div class="d-flex align-items-center justify-content-center row min-vh-100">
            <form id="siteLogin" class="account-box login-box" @keyup.enter="signIn">
                <img
                    src="/assets/images/one-beyond-logo-black.svg"
                    height="60"
                    class="mx-auto d-block logo"
                >
                <h1 class="text-center">{{$t('title')}}</h1>

                <div class="form-group">
                    <label for="username">{{$t('field.username')}}</label>
                    <input type="text" v-model="username" name="username" class="form-control">
                    <div
                        v-show="!username"
                        class="invalid-feedback"
                    >
                        {{$t('message.userNameRequired')}}
                    </div>
                </div>
                <div class="form-group">
                    <label for="password">{{$t('field.password')}}</label>
                    <input type="password" v-model="password" name="password" class="form-control">
                    <div
                        v-show="!password"
                        class="invalid-feedback"
                    >
                        {{$t('message.passwordRequired')}}
                    </div>
                </div>
                <div class="form-group text-center">
                    <div class="custom-control custom-checkbox">
                        <input
                            type="checkbox"
                            class="custom-control-input"
                            id="rememberMe"
                            name="rememberMe"
                            value="true"
                            v-model="rememberMe"
                        >
                        <label
                            class="custom-control-label"
                            for="rememberMe"
                        >{{$t('field.rememberMe')}}</label>
                    </div>
                </div>
                <div class="form-group text-center">
                    <button
                        class="btn btn-primary"
                        id="submit-btn"
                        @click="signIn"
                        :disabled="signingIn"
                    >
                        {{$t('button.signIn')}}
                    </button>
                </div>
                <p class="text-center">
                    <router-link to="ForgotPassword">{{$t('button.forgottenPassword')}}</router-link>
                </p>

                <p class="text-center" style="margin-top:30px;">
                    <span>{{$t('button.or')}}</span>
                    <a>{{$t('button.Azure')}}</a>
                </p>

                <div v-if="errorMsg">
                    <div class="alert alert-danger">
                        {{errorMsg}}
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import dictionary from "@js/localizations/resources/components/signIn";
    import { SignInStatus } from "@js/dataModels/auth/signInStatus";
    import { SignInResult } from "@js/dataModels/auth/signInResult";
    import AccountApiClient from "@js/api/accounts/accountApiClient";
    import { SignInRequest } from "@js/dataModels/auth/signInRequest";
    import LocalSessionStorage from "@js/stores/localSessionStorage";

    @Component({
        name: "SignIn",
        i18n: {
            messages: dictionary
        }
    })
    export default class SignIn extends Vue {
        public signingIn: boolean = false;
        public username: string = "";
        public password: string = "";
        public rememberMe: boolean = false;
        public errorMsg: string = "";
        private accountApiClient: AccountApiClient;

        constructor() {
            super();
            this.accountApiClient = new AccountApiClient();
        }

        async signIn(): Promise<void> {
            this.signingIn = true;

            this.errorMsg = "";
            const defaultError: string = "An error occured while trying to log you in.";

            const userCredentials = new SignInRequest(this.username, this.password, this.rememberMe);

            try {
                const data: SignInResult = await this.accountApiClient.basicSignIn(userCredentials);

                if (data.status === SignInStatus.Success) {
                    LocalSessionStorage.setUserAuthenticated(true);
                    window.location.href = `${(window as any).location.origin}/admin/`;
                }
                else {
                    this.errorMsg = data.statusMessage ?? defaultError;
                }
            }
            catch {
                this.errorMsg = defaultError;
            }
            finally {
                this.signingIn = false;
            }
        }
    }
</script>