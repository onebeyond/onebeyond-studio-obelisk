import { SignInStatus } from '@js/dataModels/auth/signInStatus';

export class SignInResult {
    status: SignInStatus | undefined;
    statusMessage: string | undefined;
}