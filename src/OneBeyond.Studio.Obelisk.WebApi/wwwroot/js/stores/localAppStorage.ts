export default class LocalAppStorage {
    public static readonly APPLICATION_NAME: string = 'Obelisk';

    public static setValueForKey(key: string, value: string): void {
        window.localStorage.setItem(this.getKey(key), value);
    }

    public static getValueForKey(key: string): string | null {
        return window.localStorage.getItem(this.getKey(key));
    }

    private static getKey(key: string): string {
        return `${this.APPLICATION_NAME}-${key}`;
    }
}
