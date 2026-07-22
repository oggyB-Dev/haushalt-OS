import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth-guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./features/login/login').then(m => m.Login),
    },
    {
        path: 'register',
        loadComponent: () => import('./features/register/register').then(m => m.Register),
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () => import('./core/layout/shell').then(m => m.Shell),
        children: [
            { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
            {
                path: 'dashboard',
                loadComponent: () => import('./features/dashboard/dashboard').then(m => m.Dashboard),
            },
        ],
    },
];