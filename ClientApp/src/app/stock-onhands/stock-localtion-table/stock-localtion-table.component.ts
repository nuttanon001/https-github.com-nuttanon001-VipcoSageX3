import { Component, OnInit, Input } from '@angular/core';
import { StockLocation } from '../shared/stock-location.model';
import { MyPrimengColumn } from '../../shared/column.model';

@Component({
  selector: 'app-stock-localtion-table',
  templateUrl: './stock-localtion-table.component.html',
  styleUrls: ['./stock-localtion-table.component.scss']
})
export class StockLocaltionTableComponent implements OnInit {

  constructor() { }

  //Parameter
  @Input() datasource: Array<StockLocation>;
  columns: Array<MyPrimengColumn>;

  ngOnInit() {
    this.columns = new Array;
    this.columns = [
      { field: 'QuantityString', header: 'StockOnLocation', width: 150 },
      { field: 'LocationCode', header: 'Location', width: 100 },
      { field: 'Uom', header: 'Uom', width: 75 },
    ];
  }

}
