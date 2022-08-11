// MODULES
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { Ng2ImgMaxModule } from 'ng2-img-max';

import { UiSwitchModule } from 'ngx-toggle-switch';

import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

// COMPONENTS
import { AppComponent } from './app.component';
import { LoginComponent } from '@components/common/login/login.component';
import { Error401Component } from '@components/common/error-page/error401/error-401.component';
import { Error404Component } from '@components/common/error-page/error404/error-404.component';

// SERVICES
import { CookieService } from 'ngx-cookie-service';
import { AuthenticationService } from '@services/auth/authentication.service';
import { LoadingIndicatorService} from '@services/loading-indicator.service';

import { CommonViewService } from '@services/common/common-view.service';


// INTERCEPTORS
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RequestInterceptor } from '@library/_interceptor/request-interceptor.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    Error401Component,
    Error404Component
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    Ng2ImgMaxModule,
    UiSwitchModule,
    Ng4LoadingSpinnerModule.forRoot()

  ],
  providers: [CookieService,
      LoadingIndicatorService,
      AuthenticationService,
      CommonViewService,
                {
                provide: HTTP_INTERCEPTORS,
                useClass: RequestInterceptor,
                multi: true,
              }],
  bootstrap: [AppComponent]
})
export class AppModule { }
