import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit {

  @Input() home: boolean;

  @Input() latitude;

  @Input() longitude;
  zoom = 4;

  constructor() { }

  ngOnInit(): void {
  }

}
