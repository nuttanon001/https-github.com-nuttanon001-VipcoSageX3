// Angular Core
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
// Services
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { downloadFile } from '../../payments/shared/payment.service';
// Models
import { StockOnhand } from './stock-onhand.model';
import { Scroll } from '../../shared/scroll.model';
// Rxjs
import { Observable } from 'rxjs';

@Injectable()
export class StockOnhandService extends BaseRestService<StockOnhand> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/StockOnHand/",
      "StockOnHandService",
      "ItemCode",
      httpErrorHandler
    )
  }

  getXlsx(scroll: Scroll): Observable<any> {
    let url: string = this.baseUrl + "GetReport/";

    return this.http.post(url, JSON.stringify(scroll), {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      }),
      responseType: 'blob' // <-- changed to blob 
    }).map(res => downloadFile(res, 'application/xlsx', 'export.xlsx'));
  }
}
