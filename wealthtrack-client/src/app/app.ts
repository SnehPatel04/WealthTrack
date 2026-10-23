import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  private readonly authService = inject(AuthService);

  protected readonly isLoggedIn = this.authService.isLoggedIn;
  protected readonly firstName  = this.authService.userFirstName;
  protected readonly userRole   = this.authService.userRole;

  protected logout() {
    this.authService.logout();
  }
}
