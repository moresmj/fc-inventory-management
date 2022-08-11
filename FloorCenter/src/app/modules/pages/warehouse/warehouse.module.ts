// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RouterModule } from '@angular/router';
import { WarehouseRoutingModule } from '@pageRoutes/warehouse/warehouse-routing.module';
import { SharedModule } from '@modules/pages/shared/shared.module';


// SERVICES
import { BaseService } from '@services/base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { PagerService } from '@services/common/pager.service';
import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { UserService } from '@services/user/user.service';
import { ProductService } from '@services/product/product.service';
import { OrderRequestService } from '@services/order_request/order-request.service';
import { StockService } from '@services/stock/stock.service';
import { WarehouseInventoriesService } from '@services/warehouse/warehouse-inventories.service';
import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreRequestService } from '@services/store-request/store-request.service';

// FILTERS


// COMPONENTS
import { WarehouseBaseComponent } from '@baseComponents/warehouse/warehouse-base.component';
import { DashboardComponent } from '@warehouse/Dashboard/dashboard.component';
import { DeliveryRequestDetailsComponent } from '@warehouse/delivery-request-management/details/delivery-request-details.component';
import { DeliveryRequestListComponent } from '@warehouse/delivery-request-management/list/delivery-request-list.component';
import { DeliveryRequestAdvancedSearchComponent } from '@warehouse/delivery-request-management/advanced-search/delivery-request-advanced-search.component';
import { StockRegistrationComponent } from '@w_srm/registration/stock-regist/stock-registration.component';
import { StockRegistrationListComponent } from '@w_srm/registration/list/stock-registration-list.component';
import { RecievedListComponent } from '@w_srm/recieved-list/list/recieved-list.component';
import { RecievedListAdvanceSearchComponent } from '@w_srm/recieved-list/advance-search/rl-advance-search.component';
import { RecievedListDetailsComponent} from '@w_srm/recieved-list/details/recieved-list-details.component';
import { ImportPhysicalCountComponent } from '@w_im/import-physical-count/ipc.component';


// COMMON COMPONENTS
import { ShowErrorsComponent } from '@components/Show-errors/show-errors.component';
import { PagerComponent } from '@pager/pager.component';
import { LoadingSpinnerComponent } from '@components/Loader/loader.component';
/*import { LoaderService } from '@services/common/loader.service';*/


// VALIDATORS


@NgModule({
  declarations: [       
    WarehouseBaseComponent,
    DashboardComponent,
    DeliveryRequestListComponent,
    DeliveryRequestDetailsComponent,
    DeliveryRequestAdvancedSearchComponent,
    StockRegistrationComponent,
    StockRegistrationListComponent,
    RecievedListComponent,
    RecievedListAdvanceSearchComponent,
    RecievedListDetailsComponent,
    ImportPhysicalCountComponent,
   // LoadingSpinnerComponent

  ],
  imports: [
  	CommonModule,
    WarehouseRoutingModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    SharedModule
  ],
  providers: [
   BaseService, 
    CommonViewService, 
    PagerService, 
    StoreService, 
    WarehouseService,
    ProductService,
    UserService,
    OrderRequestService,
    StockService,
     WarehouseInventoriesService,
     StoreRequestService,
   StoreInventoriesService 

    ]
})
export class WarehouseModule { }