import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "@library/_guards/auth.guard";

import { MainBaseComponent } from "@baseComponents/main/main-base.component";
import { DashboardComponent } from "@main/dashboard/dashboard.component";

import { UserListComponent } from "@m_transaction/user/list/user-list.component";
import { UserDealerListComponent } from "@m_transaction/user-dealer/list/user-dealer-list.component";

import { StoreListComponent } from "@main/transaction/store/list/store-list.component";
import { WarehouseListComponent } from "@m_transaction/warehouse/list/warehouse-list.component";
import { ItemListComponent } from "@m_transaction/item/list/item-list.component";
import { ApproveRequestOrdersListComponent } from "@m_transaction/approve-requests/orders/list/ar-orders-list.component";
import { ApproveRequestReturnsListComponent } from "@m_transaction/approve-requests/returns/list/ar-returns-list.component";
import { ApproveRequestTransfersListComponent } from "@m_transaction/approve-requests/transfers/list/ar-transfers-list.component";

import { StoreInventoryListComponent } from "@m_im/store-inventory/list/si-list.component";
import { WarehouseInventoryListComponent } from "@m_im/warehouse-inventory/list/wi-list.component";

import { StoreStockHistoryComponent } from "@m_im/store-inventory/_stock-history/s-stock-history.component";
import { WarehouseStockHistoryComponent } from "@m_im/warehouse-inventory/_stock-history/w-stock-history.component";

import { AdjustInventoryListComponent } from "@m_transaction/adjust-inventory/list/ai-list.component";
import { AdjustInventoryDetailsComponent } from "@m_transaction/adjust-inventory/_approve/ai-details.component";

import { AdjustWarehouseInventoryListComponent } from "@m_transaction/adjust-inventory/warehouse/list/w-ai-list.component";
import { AdjustWarehouseInventoryDetailsComponent } from "@m_transaction/adjust-inventory/warehouse/_approve/w-ai-details.component";

import { InventoryMonitoringListComponent } from "@m_reports/im-incoming/list/im-incoming-list.component";
import { InventoryMonitoringOutgoingListComponent } from "@m_reports/im-outgoing/list/im-outgoing-list.component";
import { StoreRTVListComponent } from "@m_reports/store-rtv/list/s-rtv-list.component";

import { AssignDrListComponent } from "@m_transaction/assign-dr/list/assign-dr-list.component";

import { AuditTrailListComponent } from "@main/audit-trail/list/audit-trail-list.component";
import { CompanyListComponent } from "@m_transaction/company/list/company-list.component";

import { ApproveRequestChangeTonalityListComponent } from "@m_transaction/approve-requests/change-tonality/list/change-tonality-list.component";

import { ApproveAdvanceOrderListComponent } from "@m_transaction/approve-requests/advance-orders/list/advance-orders-list.component";
import { ApproveRequestOrdersDealerListComponent } from "@m_transaction/approve-requests/orders-dealer/list/ar-orders-dealer-list.component";

const mainRoutes: Routes = [
  {
    path: "",
    component: MainBaseComponent,
    canActivate: [AuthGuard],
    children: [
      { path: "", redirectTo: "dashboard", pathMatch: "full" },
      { path: "dashboard", component: DashboardComponent },
      { path: "add_user", component: UserListComponent },
      { path: "add_user-dealer", component: UserDealerListComponent },
      { path: "store_list", component: StoreListComponent }, //add_store.html
      { path: "add_warehouse", component: WarehouseListComponent },
      { path: "add_item", component: ItemListComponent }, // add_item.html
      { path: "order_request", component: ApproveRequestOrdersListComponent },
      { path: "return_request", component: ApproveRequestReturnsListComponent },
      {
        path: "transfer_request",
        component: ApproveRequestTransfersListComponent
      },
      { path: "store_inventory_list", component: StoreInventoryListComponent },
      {
        path: "warehouse_inventory_list",
        component: WarehouseInventoryListComponent
      },
      {
        path: "store_inventory_list/item-history",
        component: StoreStockHistoryComponent
      },
      {
        path: "warehouse_inventory_list/item-history",
        component: WarehouseStockHistoryComponent
      },
      {
        path: "adjust_inventory_list",
        component: AdjustInventoryListComponent
      },
      {
        path: "adjust_inventory_list/details/:id",
        component: AdjustInventoryDetailsComponent
      },
      {
        path: "w_adjust_inventory_list",
        component: AdjustWarehouseInventoryListComponent
      },
      {
        path: "w_adjust_inventory_list/details/:id",
        component: AdjustWarehouseInventoryDetailsComponent
      },
      {
        path: "incoming_monitoring",
        component: InventoryMonitoringListComponent
      },
      {
        path: "outgoing_monitoring",
        component: InventoryMonitoringOutgoingListComponent
      },
      { path: "assign_dr_list", component: AssignDrListComponent },
      { path: "audit_trail_list", component: AuditTrailListComponent },
      { path: "store-rtv", component: StoreRTVListComponent },
      { path: "company_list", component: CompanyListComponent },
      {
        path: "change_tonality",
        component: ApproveRequestChangeTonalityListComponent
      },
      { path: "advance_order", component: ApproveAdvanceOrderListComponent },
      { path: "order_dealer_request", component: ApproveRequestOrdersDealerListComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(mainRoutes)],
  exports: [RouterModule]
})
export class MainRoutingModule {}
