import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService    = inject(AuthService);
  const router         = inject(Router);
  const requiredRole   = route.data['role'] as string;
  const currentRole    = authService.userRole();

  if (currentRole === requiredRole) {
    return true;
  }

  return router.createUrlTree(['/unauthorized']);
};
