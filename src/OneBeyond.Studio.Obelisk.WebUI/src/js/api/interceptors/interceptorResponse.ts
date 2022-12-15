/**
 * The response given by an interceptor
 * @param canContinue should be true if the next interceptor can be called
 * @param suppressResponseError should be true if we don't want to throw error in case of response status different from 2xx
 * @param errorMessage a string with an error message that will be thrown, if present
 */
export default class InterceptorResponse {
    public readonly canContinue: boolean;
    public readonly suppressResponseError: boolean;
    public readonly errorMessage: string | undefined;

    constructor(canContinue: boolean, suppressResponseError: boolean, errorMessage?: string) {
        this.canContinue = canContinue;
        this.suppressResponseError = suppressResponseError;
        if (this.canContinue && !!this.errorMessage) {
            throw Error("Cannot set an error message if the interceptor won't block the next call");
        }

        this.errorMessage = errorMessage;
    }
}