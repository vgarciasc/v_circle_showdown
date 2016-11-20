using System.Collections.Generic;

public class Subject {
    List<IObserver> observers_ = new List<IObserver>();

    public void addObserver(IObserver observer) {
        observers_.Add(observer);
    }

    public void removeObserver(IObserver observer) {
        observers_.Remove(observer);
    }

    public void notify(Event ev) {
        for (int i = 0; i < observers_.Count; i++)
            observers_[i].onNotify(ev);
    }
}