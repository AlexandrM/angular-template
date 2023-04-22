import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PagesRoutingModule } from './pages-routing.module';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ComponentsModule } from '../shared/components/components.module';

@NgModule({
	declarations: [
		UserProfileComponent,
	],
	imports: [
		CommonModule,
		ComponentsModule,
		FormsModule,
		ReactiveFormsModule,
		PagesRoutingModule,
		MatFormFieldModule,
		MatInputModule,
		MatIconModule,
		MatButtonModule,
	],
})
export class PagesModule {
}
