# Worked Example: Create User And Password Setup Flow

This is the first reference flow because it crosses controller input, auth setup, domain/application layers, email delivery, and password-link generation.

## Main files

- `src/OneBeyond.Studio.Obelisk.WebApi/Controllers/UsersController.cs`
- `src/OneBeyond.Studio.Obelisk.Application/Features/Users/Dto/CreateUserDto.cs`
- `src/OneBeyond.Studio.Obelisk.Domain/Features/Users/Commands/CreateUser.cs`
- `src/OneBeyond.Studio.Obelisk.Application/Features/Users/CommandHandlers/CreateUserHandler.cs`
- `src/OneBeyond.Studio.Obelisk.Domain/Features/Users/Entities/User.cs`
- `src/OneBeyond.Studio.Obelisk.Domain/Features/Users/Commands/SendResetPasswordEmail.cs`
- `src/OneBeyond.Studio.Obelisk.Application/Features/Users/CommandHandlers/SendResetPasswordEmailHandler.cs`
- `src/OneBeyond.Studio.Obelisk.WebApi/Helpers/ClientApplicationLinkGenerator.cs`
- `src/OneBeyond.Studio.Obelisk.WebApi/Controllers/AuthController.cs`

## Create user path

1. `UsersController.CreateUser` receives `CreateUserDto`.
2. The controller first creates a login with `CreateLogin`.
3. The controller builds a `CreateUser` domain command and includes a set-password URL from `ClientApplicationLinkGenerator`.
4. `CreateUserHandler` handles the command in `Application`.
5. The handler calls `User.Apply(command)` and persists through `IRWRepository<UserBase, Guid>`.

## Reset password path

1. `UsersController.ResetPassword` or `AuthController.ForgotPassword` triggers reset-token generation.
2. The controller creates a reset-password URL through `ClientApplicationLinkGenerator`.
3. `SendResetPasswordEmail` is dispatched.
4. `SendResetPasswordEmailHandler` loads the user, gets the predefined email template, renders it, and sends email through the configured sender.

## Why this flow matters

- It shows the controller-to-mediator pattern.
- It shows commands in `Domain` and handlers in `Application`.
- It shows how the backend depends on `ClientApplication` configuration rather than a coupled frontend project.
- It shows how environment-specific email behavior fits into a real user-facing flow.
- It shows why auth and email changes are review-heavy even when the code path looks straightforward.

## Reuse guidance

Use this example as the reference when you need to add:

- a new write endpoint
- link-generation behavior tied to client URLs
- email-triggering application behavior
- auth-adjacent account management functionality
