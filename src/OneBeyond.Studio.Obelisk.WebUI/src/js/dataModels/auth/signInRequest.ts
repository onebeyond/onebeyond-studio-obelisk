export class SignInRequest {
    constructor(readonly username: string, readonly password: string, readonly rememberMe: boolean) { }
}