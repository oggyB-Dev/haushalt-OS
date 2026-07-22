import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth-service';
import { Router, RouterLink } from '@angular/router';
import { email, form, FormField, minLength, required, submit } from '@angular/forms/signals';


@Component({
  selector: 'app-register',
  imports: [FormField, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly registerModel = signal({
    displayName: "",
    email: "",
    password: "",
    inviteCode: "",
  });

  protected readonly registerForm = form(this.registerModel, (path) => {
    required(path.displayName, { message: 'Anzeigename ist erforderlich.' });
    minLength(path.displayName, 2, { message: 'Mindestens 2 Zeichen.' });
    required(path.email, { message: 'E-Mail ist erforderlich.' });
    email(path.email, { message: 'Bitte eine gültige E-Mail eingeben.' });
    required(path.password, { message: 'Passwort ist erforderlich.' });
    minLength(path.password, 8, { message: 'Mindestens 8 Zeichen.' });
  });

  protected readonly serverError = signal<string | null>(null);

  protected async onSubmit(event: Event): Promise<void> {
    event.preventDefault();
    this.serverError.set(null);

    await submit(this.registerForm, async () => {
      try {
        await this.authService.register(this.registerModel());
      } catch (error) {
        const problem = error as { error?: { title?: string } };
        this.serverError.set(problem.error?.title ?? 'Registrierung fehlgeschlagen.');
        return;
      }
      await this.router.navigate(['/dashboard']);
    });
  }
}
