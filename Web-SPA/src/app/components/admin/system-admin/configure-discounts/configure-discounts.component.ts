import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SystemAdminService } from 'src/services/system-admin.service';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-configure-discounts',
  templateUrl: './configure-discounts.component.html',
  styleUrls: ['./configure-discounts.component.scss']
})
export class ConfigureDiscountsComponent implements OnInit {

  adminId: number;
  companyType: string;

  form: FormGroup;

  errorBonus = false;
  errorDiscount = false;

  showErrorMsg = true;
  errorMsg = 'Error 404 bla bla';

  data: {
    bonus: any,
    discount: any,
  };

  isOk = false;

  constructor(private route: ActivatedRoute, private router: Router,
              public adminService: SystemAdminService, private formBuilder: FormBuilder,
              private toastr: ToastrService) {
      route.params.subscribe(params => {
      this.adminId = params.id;
      });
  }

  ngOnInit(): void {
    this.getBonusAndDiscount();
  }

  getBonusAndDiscount() {
    this.adminService.getBonusAndDiscount().subscribe(
      (res: any) => {
        this.data = {
          bonus: res.bonus,
          discount: res.discount
        };
        this.initform();
        this.isOk = true;
      },
      err => {
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log(err);
          // this.toastr.error('Incorrect discount or password.', 'Authentication failed.');
          this.toastr.error(err.statusText, 'Error!');
        } else {
          console.log(err);
          console.log(err.status);
          this.toastr.error(err.error, 'Error!');
        }
      }
    );
  }

  onSubmit() {
    if (this.validateForm()) {
      this.data = {
        bonus: this.form.controls.bonus.value,
        discount: this.form.controls.discount.value,
      };
      this.adminService.configureBonusAndDiscount(this.data).subscribe(
        (res: any) => {
          this.toastr.success('Success!');
          this.router.navigate(['/system-admin/' + this.adminId]);
        },
        err => {
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            console.log(err);
            // this.toastr.error('Incorrect discount or password.', 'Authentication failed.');
            this.toastr.error(err.error, 'Error!');
          } else {
            console.log(err);
            console.log(err.status);
            this.toastr.error(err.error, 'Error!');
          }
        }
      );
    }
  }

  initform() {
    this.form = this.formBuilder.group({
      bonus: [this.data.bonus, Validators.required],
      discount: [this.data.discount, Validators.required],
   });
  }

  validateForm() {
    let retVal = true;
    if (this.form.controls.bonus.invalid) {
      this.errorBonus = true;
      retVal = false;
    }
    if (this.form.controls.discount.invalid) {
      this.errorDiscount = true;
      retVal = false;
    }
    return retVal;
  }

  onExit() {
    this.router.navigate(['/system-admin/' + this.adminId]);
  }

}
