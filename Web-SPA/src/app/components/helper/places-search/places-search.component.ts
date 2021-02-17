import { Component, OnInit, ViewChild, ElementRef, NgZone, Input, Output, EventEmitter } from '@angular/core';
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { Address } from 'src/app/entities/address';

@Component({
  selector: 'app-places-search',
  templateUrl: './places-search.component.html',
  styleUrls: ['./places-search.component.scss']
})
export class PlacesSearchComponent implements OnInit {

  latitude: number;
  longitude: number;
  zoom: number;
  address: string;
  placePhoto: any;


  private geoCoder;
  // @Input() oneColumn = false;
  @Input() left = false;
  @Input() right = false;
  @Input() pickUpLocation = false;
  @Input() searchDestination = false;
  @Input() addFlight = false;
  @Input() pickAirlineLocation = false;
  @Input() addAirlineAddress = false;
  @Input() airlineAddress: string;
  @Input() myLocation: Address;
  @Input() error = false;
  @Output() focused = new EventEmitter<boolean>();
  @Output() inputValue = new EventEmitter<string>();
  @Output() inputValueLocation = new EventEmitter<string>();
  @Input() disabled = false;

  @ViewChild('search')
  public searchElementRef: ElementRef;

  @Output() cityName = new EventEmitter<string>();


  constructor(
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone
  ) { }


  ngOnInit() {

    // load Places Autocomplete
    this.mapsAPILoader.load().then(() => {
      this.setCurrentLocation();
      this.geoCoder = new google.maps.Geocoder;

      const autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement, {
        types: ['geocode'] // geocode
      });
      autocomplete.addListener('place_changed', () => {
        this.ngZone.run(() => {
          // get the place result
          const place: google.maps.places.PlaceResult = autocomplete.getPlace();

          // verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }
          // set latitude, longitude and zoom
          this.latitude = place.geometry.location.lat();
          this.longitude = place.geometry.location.lng();
          this.address = place.formatted_address;
          this.zoom = 12;
          this.placePhoto = place.photos[1].getUrl({maxWidth: 165, maxHeight: 112});

          const separator = (this.address.split(',')[1] === undefined) ? ' - ' : ', ';

          const sendData = {
            latitude: this.latitude,
            longitude: this.longitude,
            city: this.address.split(separator)[0],
            short_name: place.address_components[0].short_name,
            state: this.address.split(separator)[1],
            placePhoto: this.placePhoto
          };
          if (this.pickAirlineLocation) {
            this.inputValueLocation.emit((<HTMLInputElement> document.getElementById('pickAirlineLocation')).value);
          }
          if (this.addFlight) {
            this.inputValue.emit((<HTMLInputElement> document.getElementById('addFlight')).value);
          }
          if (this.addAirlineAddress) {
            this.inputValue.emit((<HTMLInputElement> document.getElementById('addAirlineAddress')).value);
          }
          if (this.pickUpLocation) {
            this.inputValue.emit((<HTMLInputElement> document.getElementById('pickUpLocation')).value);
          }
          if (!this.pickUpLocation && !this.searchDestination && !this.addFlight && !this.pickAirlineLocation && !this.addAirlineAddress) {
            this.inputValue.emit((<HTMLInputElement> document.getElementById('dropOffLocation')).value);
          }
          this.cityName.emit(JSON.stringify(sendData));
        });
      });
    });
  }

  // Get Current Location Coordinates
  private setCurrentLocation() {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.latitude = position.coords.latitude;
        this.longitude = position.coords.longitude;
        this.zoom = 8;
        this.getAddress(this.latitude, this.longitude);
      });
    }
  }


  markerDragEnd($event: MouseEvent) {
    this.latitude = $event.coords.lat;
    this.longitude = $event.coords.lng;
    this.getAddress(this.latitude, this.longitude);
  }

  getAddress(latitude, longitude) {
    this.geoCoder.geocode({ location: { lat: latitude, lng: longitude } }, (results, status) => {
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 12;
          this.address = results[0].formatted_address;
        } else {
          window.alert('No results found');
        }
      } else {
        window.alert('Geocoder failed due to: ' + status);
      }

    });
  }

  onFocus() {
    this.focused.emit(true);
  }

  onChange() {
    const value = (<HTMLInputElement> document.getElementById('addFlight')).value;
    this.inputValue.emit(value);
  }

  onChangePickUpLocation() {
    const value = (<HTMLInputElement> document.getElementById('pickUpLocation')).value;
    this.inputValue.emit(value);
  }

  onChangeDropOffLocation() {
    const value = (<HTMLInputElement> document.getElementById('dropOffLocation')).value;
    this.inputValue.emit(value);
  }

  onChangeLocation() {
    const value = (<HTMLInputElement> document.getElementById('pickAirlineLocation')).value;
    this.inputValueLocation.emit(value);
  }

  onChangeAddAirline() {
    const value = (<HTMLInputElement> document.getElementById('addAirlineAddress')).value;
    this.inputValue.emit(value);
  }
}
