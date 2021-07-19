import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Airline } from 'src/app/entities/airline';
import { AirlineService } from 'src/services/airline.service';
import { ActivatedRoute } from '@angular/router';
import { Address } from 'src/app/entities/address';
import { CarRentService } from 'src/services/car-rent.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

class ImageSnippet {
  pending = false;
  status = 'init';

  constructor(public src: string, public file: File) {}
}

@Component({
  selector: 'app-company-profile',
  templateUrl: './company-profile.component.html',
  styleUrls: ['./company-profile.component.scss']
})
export class CompanyProfileComponent implements OnInit {

  companyType: string;

  adminId: number;
  airlineId: number;
  racId: number;

  companyFields: {name: string, location: Address, about: string};

  form: FormGroup;

  location: Address;
  lastLocationString: string;
  lastGoodLocationString: string;

  errorName = false;
  errorAbout = false;
  errorLocation = false;

  formOk = false;

  selectedFile: ImageSnippet;

  imageToShow: any;

  img;


  constructor(private route: ActivatedRoute,
              private airlineService: AirlineService,
              private racService: CarRentService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.adminId = params.id;
      this.companyType = params.type;
    });
  }

  private onSuccess() {
    this.selectedFile.pending = false;
    this.selectedFile.status = 'ok';
  }

  private onError() {
    this.selectedFile.pending = false;
    this.selectedFile.status = 'fail';
    this.selectedFile.src = '';
  }

  ngOnInit(): void {
    this.companyFields = {
      name: '',
      location: new Address(),
      about: ''
    };
    this.initForm();
    if (this.companyType === 'airline-profile') {
      const air1 = this.airlineService.getAirline(this.adminId).subscribe(
        (data: any) => {
          this.airlineId = data.airlineId;
          this.companyFields = {
            name: data.name,
            location: new Address(data.address.city, data.address.state, data.address.lon, data.address.lat),
            about: data.promoDescription
          };
          this.img = this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${data.logoUrl}`);
          this.form.patchValue({
            name: data.name,
            about: data.promoDescription
          });
          this.formOk = true;
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    } else if (this.companyType === 'rac-profile') {
      const air1 = this.racService.getRAC().subscribe(
        (data: any) => {
          this.racId = data.racId;
          this.companyFields = {
            name: data.name,
            location: new Address(data.address.city, data.address.state, data.address.lon, data.address.lat),
            about: data.about
          };
          this.img = this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${data.logoUrl}`);
          this.form.patchValue({
            name: data.name,
            about: data.about
          });
          this.formOk = true;
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    }
  }

  onFileChanged(imageInput: any) {
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {

      this.selectedFile = new ImageSnippet(event.target.result, file);

      this.selectedFile.pending = true;
      if (this.companyType === 'airline-profile') {
        const body = {
          image: this.selectedFile.file,
        };
        this.airlineService.changePhoto(body).subscribe(
          (res) => {
            this.onSuccess();
            this.toastr.success('Success!.');
          },
          err => {
            this.toastr.error(err.statusText, 'Error.');
          });
      } else if (this.companyType === 'rac-profile') {
        const body = {
          image: this.selectedFile.file,
        };
        this.racService.changeLogo(body).subscribe(
          (res) => {
            this.onSuccess();
            this.toastr.success('Success!.');
          },
          err => {
            this.toastr.error(err.statusText, 'Error.');
          });
      }
    });

    reader.readAsDataURL(file);
  }

  initForm() {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      about: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    if (this.form.valid && this.validateAddress()) {
      if (this.companyType === 'airline-profile') {
        const data = {
          id: this.airlineId,
          name: this.form.controls.name.value,
          address: (this.location === undefined) ? this.companyFields.location : this.location,
          promoDescription: this.form.controls.about.value,
        };
        // OVDE
        this.airlineService.editAirline(data).subscribe(
          (res: any) => {
            this.toastr.success('Success!.');
          },
          err => {
            console.log(err);
            console.log(err.error);
            this.toastr.error(err.error, 'Error.');
          }
        );
      } else if (this.companyType === 'rac-profile') {
        const data = {
          id: this.racId,
          name: this.form.controls.name.value,
          address: (this.location === undefined) ? this.companyFields.location : this.location,
          promoDescription: this.form.controls.about.value,
        };
        // OVDE
        this.racService.editRAC(data).subscribe(
          (res: any) => {
            this.toastr.success('Success!.');
          },
          err => {
            this.toastr.error(err.error, 'Error.');
          }
        );
      }
    }
  }

  validateAddress() {
    let retVal = true;
    if (this.location === undefined && this.lastLocationString === undefined) {
      return retVal;
    }
    if (this.lastLocationString === '') {
      this.location = this.companyFields.location;
      return retVal;
    }
    if (this.lastGoodLocationString !== this.lastLocationString) {
      this.errorLocation = true;
      retVal = false;
    }
    return retVal;
  }

  onLocation(value: any) {
    const obj = JSON.parse(value);
    this.location = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodLocationString = this.lastLocationString;
  }

  onLocationInputChange(value: any) {
    this.lastLocationString = value;
  }

  removeErrorClass() {
    this.errorLocation = false;
  }
}
