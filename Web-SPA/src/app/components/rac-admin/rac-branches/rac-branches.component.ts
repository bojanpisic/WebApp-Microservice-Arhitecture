import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';

@Component({
  selector: 'app-rac-branches',
  templateUrl: './rac-branches.component.html',
  styleUrls: ['./rac-branches.component.scss']
})
export class RacBranchesComponent implements OnInit {

  adminId: number;
  branches: Array<{branchId: number, city: string, state: string}>;
  indexOfPickedBranch: number;
  pickedBranchAddress: {city: string, state: string};
  showModal = false;

  searchText = '';

  // tslint:disable-next-line:max-line-length
  constructor(private routes: ActivatedRoute, private racRervice: CarRentService, private router: Router) {
    routes.params.subscribe(route => {
      this.adminId = route.id;
    });
    this.branches = [];
  }

  ngOnInit(): void {
    this.racRervice.getAdminsBranches().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const new1 = {
              branchId: element.branchId,
              city: element.city,
              state: element.state
            };
            this.branches.push(new1);
          });
        }
      },
      err => {
        console.log('dada' + err.status);
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log('400' + err);
          // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
        } else if (err.status === 401) {
          console.log(err);
        } else {
          console.log(err);
        }
      }
    );
  }

  onDelete(index: number) {
    this.indexOfPickedBranch = index;
    this.pickedBranchAddress = {city: this.branches[index].city, state: this.branches[index].state};
    this.showModal = true;
  }

  onDeleteBranch(value: boolean) {
    if (value) {
      const data = {
        id: this.branches.find(x => x.city === this.pickedBranchAddress.city).branchId
      };
      this.racRervice.deleteBranch(data).subscribe(
        (res: any) => {
          this.branches = [];
          res.forEach(element => {
            const new1 = {
              branchId: element.branchId,
              city: element.city,
              state: element.state
            };
            this.branches.push(new1);
          });
        },
        err => {
          console.log('dada' + err.status);
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            console.log(err);
          // tslint:disable-next-line: triple-equals
          } else if (err.status == 401) {
            console.log(err);
          } else {
            console.log(err);
          }
        }
      );
    }
    this.showModal = false;
  }

  addBranch(value: any) {
    const obj = JSON.parse(value);
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < this.branches.length; i++) {
      if (this.branches[i].city === obj.city && this.branches[i].state === obj.state) {
        return;
      }
    }
    const data = {
      State: obj.state,
      City: obj.city,
    };
    this.racRervice.addBranch(data).subscribe(
      (res: any) => {
        res.forEach(element => {
          if (!this.branches.find(x => x.city === element.city)) {
            const new1 = {
              branchId: element.branchId,
              city: element.city,
              state: element.state
            };
            this.branches.push(new1);
          }
        });
      },
      err => {
        console.log('dada' + err.status);
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log(err);
        // tslint:disable-next-line: triple-equals
        } else if (err.status == 401) {
          console.log(err);
        } else {
          console.log(err);
        }
      }
    );
  }

  onBranchInfo(branchId: number) {
    this.router.navigate(['rac-admin/' + this.adminId + '/cars/' + branchId]);
  }

}
