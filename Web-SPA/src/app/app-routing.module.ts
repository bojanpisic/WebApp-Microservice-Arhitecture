import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { HomeComponent } from "./components/home/home.component";
import { SignupComponent } from "./components/join/signup/signup.component";
import { SigninComponent } from "./components/join/signin/signin.component";
import { AirlinesComponent } from "./components/al-components/airlines/airlines.component";
import { AirlineInfoComponent } from "./components/al-components/airline-info/airline-info.component";
import { RentACarServiceInfoComponent } from "./components/rac-components/rent-a-car-service-info/rent-a-car-service-info.component";
import { RentACarServicesComponent } from "./components/rac-components/rent-a-car-services/rent-a-car-services.component";
import { AddFlightComponent } from "./components/airline-admin/airline-flights/add-flight/add-flight.component";
import { SpecialOffersComponent } from "./components/al-components/special-offers/special-offers.component";
import { FilterComponent } from "./components/al-components/filter/filter.component";
import { TripsComponent } from "./components/al-components/trips/trips.component";
import { CarsComponent } from "./components/rac-components/cars/cars.component";
import { ProfileComponent } from "./components/helper/profile/profile.component";
import { EditProfileComponent } from "./components/helper/profile/edit-profile/edit-profile.component";
import { FriendsComponent } from "./components/registered-user/friends/friends.component";
import { MessagesComponent } from "./components/registered-user/messages/messages.component";
import { FlightReservationComponent } from "./components/reservations/flight-reservation/flight-reservation.component";
import { AdminHomeComponent } from "./components/airline-admin/admin-home/admin-home.component";
import { AirlineDestinationsComponent } from "./components/airline-admin/airline-destinations/airline-destinations.component";
import { AirlineFlightsComponent } from "./components/airline-admin/airline-flights/airline-flights.component";
import { CompanyProfileComponent } from "./components/admin/company-profile/company-profile.component";
import { ConfigureSeatsComponent } from "./components/airline-admin/airline-flights/configure-seats/configure-seats.component";
import { RacAdminHomeComponent } from "./components/rac-admin/rac-admin-home/rac-admin-home.component";
import { RacBranchesComponent } from "./components/rac-admin/rac-branches/rac-branches.component";
import { RacCarsComponent } from "./components/rac-admin/rac-cars/rac-cars.component";
import { AddCarComponent } from "./components/rac-admin/rac-cars/add-car/add-car.component";
import { EditCarComponent } from "./components/rac-admin/rac-cars/edit-car/edit-car.component";
import { AirlineSpecialOffersComponent } from "./components/airline-admin/airline-special-offers/airline-special-offers.component";
import { AddSpecialOfferComponent } from "./components/airline-admin/airline-special-offers/add-special-offer/add-special-offer.component";
import { SystemAdminComponent } from "./components/admin/system-admin/system-admin.component";
import { AddCompanyComponent } from "./components/admin/system-admin/add-company/add-company.component";
import { AuthGuard } from "./auth/auth.guard";
import { AddSystemAdminComponent } from "./components/admin/system-admin/add-system-admin/add-system-admin.component";
import { ProfileGuard } from "./auth/profile.guard";
import { RacSpecialOffersComponent } from "./components/rac-admin/rac-special-offers/rac-special-offers.component";
import { AddCarSpecialOfferComponent } from "./components/rac-admin/add-car-special-offer/add-car-special-offer.component";
import { AllCarSpecialOffersComponent } from "./components/all-car-special-offers/all-car-special-offers.component";
import { MyCarsComponent } from "./components/registered-user/my-cars/my-cars.component";
import { MyFlightsComponent } from "./components/registered-user/my-flights/my-flights.component";
import { CarFilterComponent } from "./components/helper/car-filter/car-filter.component";
import { AirlineStatsComponent } from "./components/airline-admin/airline-stats/airline-stats.component";
import { RacStatsComponent } from "./components/rac-admin/rac-stats/rac-stats.component";
import { ConfigureDiscountsComponent } from "./components/admin/system-admin/configure-discounts/configure-discounts.component";
import { ChatsComponentComponent } from "./components/registered-user/chats/chats-component/chats-component.component";
import { ChatInfoComponent } from "./components/registered-user/chats/chat-info/chat-info.component";
import { FriendListComponent } from "./components/registered-user/chats/friends-list/friend-list/friend-list.component";
const routes: Routes = [
  {
    path: "system-admin/:id",
    children: [
      { path: "", component: SystemAdminComponent },
      {
        path: "add-system-admin",
        component: AddSystemAdminComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["Admin"] },
      },
      {
        path: "configure-discounts",
        component: ConfigureDiscountsComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["Admin"] },
      },
      //  canActivate: [AuthGuard], data: {permittedRoles: ['Admin']}
      {
        path: ":type",
        component: AddCompanyComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["Admin"] },
      },
      {
        path: "profile",
        children: [
          {
            path: "edit-profile",
            component: EditProfileComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["Admin"] },
          },
        ],
      },
    ],
  },

  {
    path: "admin/:id",
    children: [
      {
        path: "profile/edit-profile",
        component: EditProfileComponent,
        canActivate: [ProfileGuard],
        data: { permittedRoles: ["AirlineAdmin"] },
      },
      {
        path: "",
        component: AdminHomeComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["AirlineAdmin"] },
      },
      {
        path: "destinations",
        component: AirlineDestinationsComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["AirlineAdmin"] },
      },
      {
        path: "flights",
        children: [
          {
            path: "",
            component: AirlineFlightsComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["AirlineAdmin"] },
          },
          {
            path: "add-flight",
            component: AddFlightComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["AirlineAdmin"] },
          },
          {
            path: ":flight/configure-seats",
            component: ConfigureSeatsComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["AirlineAdmin"] },
          },
        ],
      },
      { path: "stats", component: AirlineStatsComponent },
      // , canActivate: [AuthGuard], data: {permittedRoles: ['AirlineAdmin']}
      {
        path: "special-offers",
        children: [
          {
            path: "",
            component: AirlineSpecialOffersComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["AirlineAdmin"] },
          },
          {
            path: "add-special-offer",
            component: AddSpecialOfferComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["AirlineAdmin"] },
          },
        ],
      },
      {
        path: ":type",
        component: CompanyProfileComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["AirlineAdmin"] },
      },
    ],
  },

  {
    path: "rac-admin/:id",
    children: [
      // tslint:disable-next-line:max-line-length
      {
        path: "profile/edit-profile",
        component: EditProfileComponent,
        canActivate: [ProfileGuard],
        data: { permittedRoles: ["RentACarServiceAdmin"] },
      },
      {
        path: "",
        component: RacAdminHomeComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["RentACarServiceAdmin"] },
      },
      {
        path: "branches",
        component: RacBranchesComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["RentACarServiceAdmin"] },
      },
      {
        path: "cars",
        children: [
          {
            path: "",
            component: RacCarsComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["RentACarServiceAdmin"] },
          },
          {
            path: "add-car",
            component: AddCarComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["RentACarServiceAdmin"] },
          },
          {
            path: ":car/edit-car",
            component: EditCarComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["RentACarServiceAdmin"] },
          },
          {
            path: ":branch",
            component: RacCarsComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["RentACarServiceAdmin"] },
          },
        ],
      },
      {
        path: "special-car-offers",
        children: [
          {
            path: "",
            component: RacSpecialOffersComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["RentACarServiceAdmin"] },
          },
          // tslint:disable-next-line:max-line-length
          {
            path: "add-car-special-offer",
            component: AddCarSpecialOfferComponent,
            canActivate: [AuthGuard],
            data: { permittedRoles: ["RentACarServiceAdmin"] },
          },
        ],
      },
      {
        path: "stats",
        component: RacStatsComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["RentACarServiceAdmin"] },
      },
      {
        path: ":type",
        component: CompanyProfileComponent,
        canActivate: [AuthGuard],
        data: { permittedRoles: ["RentACarServiceAdmin"] },
      },
    ],
  },

  { path: "", component: HomeComponent },
  { path: "signup", component: SignupComponent },
  { path: "signin", component: SigninComponent },
  { path: "signin/:id/:token", component: SigninComponent },
  { path: ":username/signin", component: SigninComponent },
  {
    path: "airlines",
    children: [
      { path: "", component: AirlinesComponent },
      {
        path: ":airline/airline-info",
        children: [
          { path: "", component: AirlineInfoComponent },
          { path: "flight-special-offers", component: SpecialOffersComponent },
        ],
      },
    ],
  },
  {
    path: "rent-a-car-services",
    children: [
      { path: "", component: RentACarServicesComponent },
      {
        path: ":rac/rent-a-car-service-info",
        children: [
          { path: "", component: RentACarServiceInfoComponent },
          {
            path: "car-special-offers",
            component: AllCarSpecialOffersComponent,
          },
        ],
      },
    ],
  },
  {
    path: "trips",
    children: [
      { path: "", component: TripsComponent },
      { path: "filter", component: FilterComponent },
    ],
  },
  {
    path: "cars",
    children: [
      { path: "", component: CarsComponent },
      { path: "car-filter", component: CarFilterComponent },
    ],
  },
  { path: "flight-special-offers", component: SpecialOffersComponent },
  { path: "car-special-offers", component: AllCarSpecialOffersComponent },

  { path: ":id/home", component: HomeComponent },
  {
    path: ":id/cars",
    children: [
      { path: "", component: CarsComponent },
      { path: "car-filter", component: CarFilterComponent },
    ],
  },
  {
    path: ":id/airlines",
    children: [
      { path: "", component: AirlinesComponent },
      {
        path: ":airline/airline-info",
        children: [
          { path: "", component: AirlineInfoComponent },
          { path: "flight-special-offers", component: SpecialOffersComponent },
        ],
      },
    ],
  },
  {
    path: ":id/rent-a-car-services",
    children: [
      { path: "", component: RentACarServicesComponent },
      {
        path: ":rac/rent-a-car-service-info",
        children: [
          { path: "", component: RentACarServiceInfoComponent },
          {
            path: "car-special-offers",
            component: AllCarSpecialOffersComponent,
          },
        ],
      },
    ],
  },
  {
    path: ":id/trips",
    children: [
      { path: "", component: TripsComponent },
      { path: "filter", component: FilterComponent },
      {
        path: "trip-reservation",
        children: [{ path: "", component: FlightReservationComponent }],
      },
    ],
  },
  {
    path: ":id/profile",
    children: [
      { path: "", component: ProfileComponent },
      { path: "edit-profile", component: EditProfileComponent },
      { path: "friends", component: FriendsComponent },
      { path: "cars", component: MyCarsComponent },
      { path: "flights", component: MyFlightsComponent },
    ],
  },
  {
    path: ":id/chat",
    children: [
      { path: "", component: ChatsComponentComponent },
      {
        path: ":conversationId/chat-info",
        component: ChatInfoComponent,
      },
    ],
  },
  { path: ":id/friends-chat", component: FriendListComponent },
  { path: ":id/messages", component: MessagesComponent },
  { path: ":id/chats", component: ChatsComponentComponent },
  { path: ":id/flight-special-offers", component: SpecialOffersComponent },
  { path: ":id/car-special-offers", component: AllCarSpecialOffersComponent },

  { path: "**", component: HomeComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
