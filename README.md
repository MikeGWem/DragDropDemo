# DragDropDemo
A demonstration of Windows Forms drag and drop operations including the creation of custom Cursors

![image](https://cloud.githubusercontent.com/assets/7824361/15215834/626b80ce-184c-11e6-9be5-202863824337.png)

Here you can see a screen image from the demo program. Addresses can be entered in the mini form on the left of the window and dragged onto the FlowLayoutPanel on the right that is acting as an "address store". To save energy, the "Fill" button will populate the form with up to 50 addresses taken randomly from an embedded list.

Addresses in the right hand panel can be dragged back to the address entry form for editing. Addresses can also be dragged up and down the list to reorder them.

We thus have two sorts of object that can be dragged for (is it) three different purposes. Some simple custom icons are created and used during the drag events.

The demo features loading data from an embedded resource (.csv file) using the Visual Basic TextFieldParser class. There are two custom DataFormats declared and used in the Drag operations. The addresses in the notional "address store" are encapsulated within multiple UserControls and these controls use Delegates to communicate with the Form. Dragging an address within the FlowLayoutPanel will trigger an automatic scroll when the scroll bar is active and the drag nears the top or bottom of the panel..

An analysis of the code will note that the demo "cheats" when determining the target location of a move within the FlowLayoutPanel and that it is possible to dodge around one or more address and expose the bug.
