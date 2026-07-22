import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from '../auth/auth-service';
import { Button } from '../../shared/ui/button';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, Button],
  templateUrl: './shell.html',
})
export class Shell {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  /** Meldet ab und kehrt zum Login zurück. */
  protected logout(): void {
    this.authService.logout();
    void this.router.navigate(['/login']);
  }
}