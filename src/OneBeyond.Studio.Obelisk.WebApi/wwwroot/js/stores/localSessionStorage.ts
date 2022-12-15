import LocalAppStorage from "./localAppStorage";

export default class LocalSessionStorage extends LocalAppStorage {
    public static readonly LAST_SERVER_REQUEST_KEY: string = 'lastServerRequestDate';

    public static updateLastServerRequestDate(): void {
        this.setValueForKey(this.LAST_SERVER_REQUEST_KEY, new Date().getTime().toString());
    }

    public static getLastServerRequestDate(): number {
        return parseInt(this.getValueForKey(this.LAST_SERVER_REQUEST_KEY) ?? '0');
    }
}