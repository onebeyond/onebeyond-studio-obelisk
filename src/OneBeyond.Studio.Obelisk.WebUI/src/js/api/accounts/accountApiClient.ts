import DcslResourceApiClient from "@js/api/dcslResourceApiClient";
import { SignInRequest } from "@js/dataModels/auth/signInRequest";
import { SignInResult } from "@js/dataModels/auth/signInResult";

//We derive from WebApiClient, not from DcslApiClient, because the Account controller does not have api folder
export default class AccountApiClient extends DcslResourceApiClient {

    constructor() {
        super("auth", "v1");
    }

    public async ping(): Promise<void> {
        await this.get("ping");
    }

    public async basicSignIn(userCredentials: SignInRequest): Promise<SignInResult> {
        const response = await this.post("basic/signin", userCredentials);
        return await response.json() as SignInResult;
    }

    public async signOut(): Promise<void> {
        await this.post("signout");
    }
}