export enum SignInStatus {
    Success = 0,
    LockedOut = 1,
    RequiresVerification = 2,
    Failure = 4,
    UnknownUser = 5,
    ConfigureTFA = 6
}