import { BrowserModule } from '@angular/platform-browser';  
import { NgModule } from '@angular/core';  

import { AppComponent } from './app.component';  
import { LoginComponent } from '@components/login/login.component';
import { PrintDrComponent } from '@components/Store/print-dr/print-dr.component';
import { AppRoutingModule } from './app-routing.module';

@NgModule({  
  declarations: [  
    AppComponent,
    LoginComponent,
    PrintDrComponent
  ],  
  imports: [  
    BrowserModule,
    AppRoutingModule
  ],  
  providers: [],  
  bootstrap: [AppComponent]  
})  
export class AppModule { }  