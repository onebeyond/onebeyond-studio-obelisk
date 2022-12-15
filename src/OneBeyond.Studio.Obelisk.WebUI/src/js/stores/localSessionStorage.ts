import LocalAppStorage from "./localAppStorage";

export default class LocalSessionStorage extends LocalAppStorage {
    public static readonly LAST_SERVER_REQUEST_KEY: string = 'lastServerRequestDate';
    public static readonly USER_IS_AUTHENTICATED_KEY: string = 'authenticatedUser';
    private static readonly API_URL_KEY: string = 'apiUrl';


    public static updateLastServerRequestDate(): void {
        this.setValueForKey(this.LAST_SERVER_REQUEST_KEY, new Date().getTime().toString());
    }

    public static getLastServerRequestDate(): number {
        return parseInt(this.getValueForKey(this.LAST_SERVER_REQUEST_KEY) ?? '0');
    }

    public static isUserAuthenticated(): boolean {
        return this.getValueForKey(this.USER_IS_AUTHENTICATED_KEY) === 'true';
    }

    public static setUserAuthenticated(authenticated: boolean): void {
        this.setValueForKey(this.USER_IS_AUTHENTICATED_KEY,  authenticated ? 'true' : 'false');
    }

    public static setCustomApiUrl(url: string): void {
        this.setValueForKey(this.API_URL_KEY, url); 
    }

    public static getCustomApiUrl(): string {
        return this.getValueForKey(this.API_URL_KEY)?.toString() ?? ""; 
    }
}