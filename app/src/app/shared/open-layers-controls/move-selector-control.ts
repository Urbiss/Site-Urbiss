import {Control} from 'ol/control';

export class MoveSelectorControl extends Control {
	constructor() {
		super({});

    const button = document.createElement('div');
    button.innerHTML = '<button title="Move control"><i class="bi bi-arrows-move"></i></button>'

		const element = document.createElement('div');
		element.className = 'ol-feature ol-control';
		element.style.marginLeft = '8px';
		element.style.marginTop = '165px';
		element.appendChild(button);

		Control.call(this, {
			element: element
		});

		button.addEventListener('click', this.moveSelector.bind(this), false);
	}

	moveSelector(): void {
		while (super.getMap().getInteractions().getLength() > 13) {
			super.getMap().getInteractions().pop();
		}
	}
}
