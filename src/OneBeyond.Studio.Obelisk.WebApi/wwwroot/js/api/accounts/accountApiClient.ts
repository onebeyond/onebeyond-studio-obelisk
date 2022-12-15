import WebApiClient from "../webApiClient";

//We derive from WebApiClient, not from DcslApiClient, because the Account controller does not have api folder
export default class AccountApiClient extends WebApiClient {

    constructor() {
        super(`${(window as any).BaseUrl}Account/`);
    }

    public async ping(): Promise<void> {
        await this.get("ping");
    }
}