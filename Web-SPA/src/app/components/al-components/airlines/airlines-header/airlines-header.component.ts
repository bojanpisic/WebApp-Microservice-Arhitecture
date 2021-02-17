import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LiteralArrayExpr } from '@angular/compiler';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-airlines-header',
  templateUrl: './airlines-header.component.html',
  styleUrls: ['./airlines-header.component.scss']
})
export class AirlinesHeaderComponent implements OnInit {

  rotateArrow = false;
  nameup = false;
  namedown = false;
  cityup = false;
  citydown = false;
  @Input() rac = false;
  @Output() sort = new EventEmitter<Array<boolean>>();
  userId;

  constructor(private route: ActivatedRoute, private router: Router) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
  }

  ngOnInit(): void {

  }

  onBack() {
    if (this.userId === undefined) {
      this.router.navigate(['/']);
    } else {
      this.router.navigate(['/' + this.userId + '/home']);
    }
  }

  sortClick() {
    this.rotateArrow = !this.rotateArrow;
  }

  sortBy() {
    this.sort.emit([this.namedown, this.nameup, this.citydown, this.cityup]);
  }

  namedownClicked() {
    if (this.nameup === true) {
      this.nameup = false;
    }
    this.namedown = !this.namedown;
    this.sortBy();
  }

  nameupClicked() {
    if (this.namedown === true) {
      this.namedown = false;
    }
    this.nameup = !this.nameup;
    this.sortBy();
  }

  citydownClicked() {
    if (this.cityup === true) {
      this.cityup = false;
    }
    this.citydown = !this.citydown;
    this.sortBy();
  }

  cityupClicked() {
    if (this.citydown === true) {
      this.citydown = false;
    }
    this.cityup = !this.cityup;
    this.sortBy();
  }

}
