import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-system-admin',
  templateUrl: './system-admin.component.html',
  styleUrls: ['./system-admin.component.scss']
})
export class SystemAdminComponent implements OnInit {

  adminId: number;

  constructor(private route: ActivatedRoute) {
    route.params.subscribe(params => {
      this.adminId = params.id;
    });
  }

  ngOnInit(): void {
  }

}
