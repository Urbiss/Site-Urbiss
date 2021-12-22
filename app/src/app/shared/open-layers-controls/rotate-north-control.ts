import { Control } from 'ol/control';

export class RotateNorthControl extends Control {
	constructor() {
		super({});


    const button = document.createElement('div');
    button.innerHTML = '<button title="Center north"><i class="bi bi-arrow-up"></i></button>'

		const element = document.createElement('div');
		element.className = 'ol-feature ol-control';
		element.style.marginLeft = '65px';
    element.style.marginTop = '137px';
		element.appendChild(button);

		Control.call(this, {
			element: element
		});

		button.addEventListener('click', this.rotateNorth.bind(this), false);
	}

	rotateNorth(): void {
		super.getMap().getView().setRotation(0);
	}
}
