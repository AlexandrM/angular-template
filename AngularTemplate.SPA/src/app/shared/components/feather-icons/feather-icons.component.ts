import { Component, Input } from '@angular/core';

@Component({
	selector: 'app-feather-icons',
	templateUrl: './feather-icons.component.html',
	styleUrls: ['./feather-icons.component.scss'],
})
export class FeatherIconsComponent {
	@Input()
	icon?: string;
	@Input()
	class?: string;

	constructor() {
		// constructor
	}
}
