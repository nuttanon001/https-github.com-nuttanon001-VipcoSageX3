import { Injectable } from '@angular/core';
import { PrAndPo } from './pr-and-po.model';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Scroll } from '../../shared/scroll.model';
import { Observable } from 'rxjs';
import { downloadFile } from '../../payments/shared/payment.service';

@Injectable()
export class PrService extends BaseRestService<PrAndPo> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseRequest/",
      "PurchaseRequestService",
      "PrNumber",
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
