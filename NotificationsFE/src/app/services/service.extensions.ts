import { Observable } from 'rxjs'; // ignore
import { HttpResponseBase } from '@angular/common/http'; // ignore

export class ServiceBase {
    protected transformOptions(options: any) {
        return Promise.resolve(options);
    }

    protected transformResult(url: string, response: HttpResponseBase, processor: (response: HttpResponseBase) => any): Observable<any> {
        return processor(response);
    }
}