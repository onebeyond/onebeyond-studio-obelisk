export default class TabPage {
    name: string;
    loaded: boolean;

    constructor(name: string) {
        this.name = name;
        this.loaded = false;
    }

    public setLoaded(): void {
        if (!this.loaded) {
            this.loaded = true;
        }
    }
}