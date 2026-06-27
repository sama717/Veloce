import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/landing/landing').then((m) => m.Landing) },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./features/auth/forgot-password/forgot-password').then((m) => m.ForgotPassword),
  },
  {
    path: 'catalog',
    loadComponent: () => import('./features/public/catalog/catalog').then((m) => m.Catalog),
  },
  {
    path: 'cars/:id',
    loadComponent: () =>
      import('./features/public/car-details/car-details').then((m) => m.CarDetails),
  },
  {
    path: 'booking/create/:id',
    loadComponent: () =>
      import('./features/public/booking-create/booking-create').then((m) => m.BookingCreate),
    canActivate: [authGuard],
  },
  {
    path: 'user',
    loadComponent: () =>
      import('./features/user/user-dashboard/user-dashboard').then((m) => m.UserDashboard),
    canActivate: [authGuard],
    children: [
      {
        path: 'bookings',
        loadComponent: () =>
          import('./features/user/user-bookings/user-bookings').then((m) => m.UserBookings),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/user/user-profile/user-profile').then((m) => m.UserProfileComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/user/user-settings/user-settings').then((m) => m.UserSettings),
      },
      { path: '', redirectTo: 'bookings', pathMatch: 'full' },
    ],
  },
  {
    path: 'my-cars',
    loadComponent: () =>
      import('./features/provider/my-cars/my-cars').then((m) => m.MyCars),
    canActivate: [authGuard],
  },
  {
    path: 'list-car',
    loadComponent: () =>
      import('./features/provider/list-car/list-car').then((m) => m.ListCar),
    canActivate: [authGuard],
  },
  {
    path: 'edit-car/:id',
    loadComponent: () =>
      import('./features/provider/edit-car/edit-car').then((m) => m.EditCar),
    canActivate: [authGuard],
  },
  {
  path: 'admin',
  canActivate: [authGuard, adminGuard],
  children: [
    {
      path: 'dashboard',
      loadComponent: () => import('./features/admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard)
    },
    {
      path: 'bookings',
      loadComponent: () => import('./features/admin/admin-bookings/admin-bookings').then(m => m.AdminBookings)
    },
    {
      path: 'dealerships',
      loadComponent: () => import('./features/admin/admin-dealerships/admin-dealerships').then(m => m.AdminDealerships)
    },
    {
      path: 'cars',
      loadComponent: () => import('./features/admin/admin-cars/admin-cars').then(m => m.AdminCars)
    },
    {
      path: '',
      redirectTo: 'dashboard',
      pathMatch: 'full'
    }
  ]
}
];