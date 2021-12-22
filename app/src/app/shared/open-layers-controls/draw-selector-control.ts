import {Control} from 'ol/control';
import {Draw} from 'ol/interaction';
import GeometryType from 'ol/geom/GeometryType';
import {Source} from 'ol/source';

export class DrawSelectorControl extends Control {
	constructor(source: Source) {
		super({});
    const button = document.createElement('div');
    button.innerHTML = '<button title="Draw control"><i class="bi bi-bounding-box-circles"></i></button>'

		const element = document.createElement('div');
		element.className = 'ol-feature ol-control';
    element.style.marginLeft = '37px';
    element.style.marginTop = '165px';
		element.appendChild(button);

		Control.call(this, {
			element: element
		});

		button.addEventListener('click', this.drawSelector.bind(this, source), false);
	}

  public onDrawEnd?: (e: any) => void
  public onDrawStart?: (e: any) => void
  public onAddFeature?: (e: any) => void

	drawSelector(source): void {
		const draw = new Draw({
			source,
			type: GeometryType.POLYGON
		});

    draw.on('drawend', (e) => {
      if (!this.onDrawEnd) return;
      this.onDrawEnd(e);
    });

    draw.on('drawstart', (e) => {
      if (!this.onDrawStart) return;
      this.onDrawStart(e);
    });

    source.on('addfeature', (e) => {
      if (!this.onAddFeature) return;
      this.onAddFeature(e);
    });

		super.getMap().addInteraction(draw);
	}
}
