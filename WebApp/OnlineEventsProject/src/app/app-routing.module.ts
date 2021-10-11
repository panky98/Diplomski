import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { HomeComponent} from './components/home-component/home-component.component';
import { LoginComponent } from './components/login-component/login.component';
import { RegistrationComponent } from './components/registration-component/registration-component.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [{ path: '', component:  HomeComponent},
                        { path: 'login', component:  LoginComponent},
                        { path: 'registration', component:  RegistrationComponent},
                        { path: "events", loadChildren: () => import("./modules/events-module/events.module").then(m => m.EventsModule),canActivate:[AuthGuard]},
                        { path: "**", redirectTo: "login" }
                      ];

@NgModule({
  imports: [RouterModule.forRoot(routes,{ preloadingStrategy: PreloadAllModules })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
