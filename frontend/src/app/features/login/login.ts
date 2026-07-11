import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth-service';
import { Router } from '@angular/router';
import { email, form, required, submit, FormField } from '@angular/forms/signals';


@Component({
  selector: 'app-login',
  imports: [FormField],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly loginModel = signal({
    email: "",
    password: ""
  });

  /** Validierungsregeln definieren */
  protected readonly loginForm = form(this.loginModel, (path) => {
    required(path.email, {message: "E-Mail ist erforderlich."});
    email(path.email, {message: "Bitte eine gültige E-Mail eingeben"});
    required(path.password, {message: "Passwort ist erforderlich."});
  });

  /** Fehlermeldung des Servers */
  protected readonly serverError = signal<string | null>(null);

  /** Führt die Anmeldung aus und navigiert bei Erfolg zum Dashboard */
  protected async onSubmit(event: Event) : Promise<void>{
    event.preventDefault();
    this.serverError.set(null);

    await submit(this.loginForm, async () => {
      try{
        await this.authService.login(this.loginModel());
      }catch (error) {
        console.error("Login fehlgeschlagen", error);
        this.serverError.set("E-Mail oder Passwort ist falsch.");
        return;
      }
      await this.router.navigate(["/dashboard"]);
    });
  }
}
