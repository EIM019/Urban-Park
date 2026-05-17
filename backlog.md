User Story 1					
As a user, I want to log into the system securely					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design login UI	High	2	Done	Responsive form with email and password fields
2	Implement authentication backend	High	5	In Progress	Session management / JWT generation
3	Add password encryption	High	3	To Do	Bcrypt hashing and verification
4	Implement "remember me"	Medium	2	To Do	Secure persistent login with cookies
5	Error handling for invalid credentials	Medium	2	To Do	User-friendly messages
6	Add two factor authentication (2FA)	Medium	6	To Do	Time based one time password (TOTP) support
7	Implement password reset flow	High	4	To Do	Email with reset link, token expiry
8	Account lockout after failed attempts	Medium	3	To Do	Lock account for 15 min after 5 failures
9	Add social login (Google, Microsoft)	Low	5	To Do	OAuth2 integration
10	Security audit logging	Medium	2	To Do	Log all login attempts (success/failure)
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 2					
As an admin, I want to create user accounts 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design create user form	High	3	Done	Name, email, password, role fields
2	Build backend API for user creation	High	4	Done	POST endpoint with validation
3	Role assignment dropdown	High	2	To Do	Populate roles dynamically
4	Validate input and prevent duplicates	Medium	3	To Do	Check email uniqueness, strong password
5	Send welcome email	Low	3	To Do	Email with account details and login link
6	Bulk user import via CSV	Medium	5	To Do	Upload CSV, validate, create users
7	Admin approval workflow	Low	4	To Do	New accounts require admin activation
8	Set password expiry policy	Medium	3	To Do	Force password change every 90 days
9	Integration with LDAP/Active Directory	Low	8	To Do	Enterprise SSO option
10	Audit trail for admin actions	Medium	2	To Do	Log who created/modified users
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 3					
As a user, I want to search available parking spaces 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design search interface	High	3	In Progress	Date/time picker, location input, search button
2	Implement availability search algorithm	High	5	To Do	Query spots not booked during selected time
3	Optimise database queries	High	4	To Do	Indexes, query performance tuning
4	Add filter options	Medium	3	To Do	Covered spots, EV charging, handicap access
5	Display results in list view	Medium	2	To Do	Spot details and prices, sortable
6	Show results on map	High	5	To Do	Interactive map with spot markers
7	Save recent searches	Low	2	To Do	Allow users to reuse previous criteria
8	Autocomplete location field	Medium	3	To Do	Google Places API integration
9	Filter by price range	Medium	2	To Do	Slider for min/max hourly rate
10	Display real-time occupancy	Medium	4	To Do	Show how many spots left in each area
11	Search by facility (e.g., near elevator)	Low	2	To Do	Additional attribute filters
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 4					
As a receptionist, I want to book visitor parking					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Create booking form for visitors	High	3	To Do	Visitor name, license plate, date/time, host
2	Link booking to visitor details	High	3	To Do	Store visitor info (company, contact)
3	Check availability and assign spot	High	4	To Do	Auto assign or manual selection
4	Send confirmation to visitor email	Medium	2	To Do	Email with booking details and QR code
5	Option for recurring bookings	Low	4	To Do	Select multiple dates for regular visitors
6	Pre register known visitors	Medium	3	To Do	Maintain visitor list for quick booking
7	Notify host of visitor arrival	Low	2	To Do	Email or in app notification
8	Print visitor parking pass	Low	3	To Do	Generate PDF with pass details
9	Integration with calendar (Outlook)	Medium	5	To Do	Add booking to host’s calendar
10	Visitor check in kiosk mode	Low	6	To Do	Tablet interface for on site check in
11	Overstay alerts	Low	3	To Do	Notify receptionist if visitor exceeds time
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 5 					
As a manager, I want priority booking 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Define priority levels in system	High	2	Done	Add priority field to user roles
2	Modify booking algorithm for priority	High	5	To Do	Managers get spots when conflicting
3	UI indicator for priority spots	Medium	3	To Do	Icon or label on reserved spots
4	Allow manager override of regular bookings	High	4	To Do	Admin override with audit trail
5	Audit log for priority assignments	Low	2	To Do	Log all priority overrides
6	Configurable priority rules	Medium	4	To Do	Admin can set priority by role/department
7	Waitlist for managers if no spots	Medium	3	To Do	Notify when spot becomes available
8	Priority booking window	Medium	3	To Do	Managers can book earlier than others
9	Reporting on priority usage	Low	3	To Do	Track how often priority is used
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 6 					
As a user, I want to view a visual carpark layout					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design graphical layout representation	High	4	Done	Wireframes of carpark grid
2	Implement interactive map	High	6	In Progress	SVG/canvas, clickable spots
3	Show available/occupied spots with colors	High	3	To Do	Green (free), red (occupied), blue (selected)
4	Zoom and pan functionality	Medium	3	To Do	Allow zooming and panning
5	Integrate with real-time data	Medium	4	To Do	Update status based on current bookings
6	Display spot details on hover	Medium	2	To Do	Show spot number, type, price
7	Highlight accessible/EV spots	Medium	2	To Do	Icons for special spots
8	Floor selector for multi level carparks	High	3	To Do	Dropdown or tabs to change level
9	Save favourite spots	Low	3	To Do	Allow users to mark preferred spots
10	3D view option	Low	8	To Do	Experimental 3D rendering
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 7 					
As admin, I want to modify carpark layouts 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Create layout editor interface	Medium	5	In Progress	Admin page with grid and spot tools
2	Allow adding/removing parking spots	Medium	4	To Do	Buttons to add new spots or delete
3	Drag and drop for spot positions	Medium	4	To Do	Visually reposition spots
4	Save layout changes to database	Medium	2	To Do	Persist layout after editing
5	Preview before publishing	Low	3	To Do	Show preview of new layout
6	Undo/redo functionality	Low	3	To Do	Allow reverting changes
7	Copy layout from another floor	Low	2	To Do	Duplicate existing layout
8	Bulk edit spot properties	Medium	4	To Do	Select multiple spots and edit type/status
9	Import layout from image	Low	5	To Do	Overlay image to trace spots
10	Version history	Low	3	To Do	Track layout changes over time
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 8 					
As admin, I want to manage system roles 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design roles management page	Medium	3	To Do	List, add, edit, delete roles
2	Create CRUD for roles	Medium	4	To Do	Backend endpoints for role operations
3	Assign permissions to roles	Medium	4	To Do	Checkbox list of permissions
4	Implement role assignment to users	Medium	3	To Do	Dropdown on user edit page
5	Audit logging for role changes	Low	2	To Do	Log who changed which role
6	Clone role	Low	2	To Do	Duplicate existing role with permissions
7	Role hierarchy (inheritance)	Low	5	To Do	Child roles inherit permissions
8	Default roles on system install	Medium	3	To Do	Predefined roles (admin, manager, user)
9	Export roles and permissions	Low	2	To Do	CSV export for documentation
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 9 					
As a user, I want to cancel a booking					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Add cancel button on booking details	Medium	2	Done	Visible for future bookings only
2	Implement cancellation backend	Medium	3	To Do	Update status to "cancelled", free spot
3	Send cancellation confirmation email	Low	2	To Do	Notify user of successful cancellation
4	Update spot availability immediately	Medium	2	To Do	Make spot available for others
5	Handle cancellation deadlines/policies	Medium	3	To Do	Enforce rules (e.g., no cancellation within 1 hour)
6	Refund processing if paid	High	5	To Do	Integrate with payment gateway for refunds
7	Allow cancellation from calendar view	Low	2	To Do	Cancel directly from "My Bookings" calendar
8	Admin override to cancel any booking	Low	2	To Do	Support team can cancel on behalf
9	Cancellation reason collection	Low	2	To Do	Optional dropdown/text field for feedback
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 10 					
As a user, I want to view my bookings 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Create "My Bookings" page	Medium	3	To Do	Accessible from user menu
2	Display list of past/upcoming bookings	Medium	3	To Do	Date, spot, status, action buttons
3	Add sorting and filtering	Low	2	To Do	Sort by date, filter by status
4	Link to booking details/cancel	Medium	2	To Do	Click to view details and cancel
5	Show status badges	Medium	2	To Do	Active, cancelled, completed
6	Calendar view alternative	Low	4	To Do	Month/week view of bookings
7	Export bookings to iCal	Low	3	To Do	Subscribe to calendar feed
8	Show booking history with changes	Low	3	To Do	Audit log of modifications
9	Print booking summary	Low	2	To Do	PDF version for records
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 11 					
As admin, I want to view booking reports 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design reports dashboard	Low	3	To Do	Charts and summary cards
2	Generate usage statistics	Low	4	To Do	Bookings per day, peak hours, utilization
3	Add export to CSV/PDF	Low	3	To Do	Download reports
4	Implement date range selector	Low	2	To Do	Filter by custom range
5	Display charts (bar, pie)	Low	4	To Do	Visual insights
6	Report on revenue (if paid)	Low	3	To Do	Income per period
7	User adoption report	Low	3	To Do	Active users, new signups
8	Spot utilisation heatmap	Low	5	To Do	Show which spots are most used
9	Scheduled email reports	Low	4	To Do	Weekly/monthly PDF reports to admins
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 12 					
As admin, I want to manage parking types 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Create parking type management UI	Low	3	To Do	List and manage types (standard, EV, handicap)
2	Define attributes (name, rate, rules)	Low	2	To Do	Fields for name, hourly rate, rules
3	CRUD operations for types	Low	3	To Do	Create, read, update, delete
4	Assign types to spots	Low	2	To Do	Dropdown when editing spots
5	Update pricing logic based on type	Low	3	To Do	Cost calculation uses type rate
6	Special rules per type	Low	3	To Do	e.g., handicap requires verification
7	Type availability restrictions	Low	3	To Do	Some types only for certain roles
8	Bulk update spot types	Low	2	To Do	Select multiple spots and change type
9					
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 13 					
As a user, I want to receive notifications about my bookings					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design notification preferences UI	Medium	3	To Do	Email/SMS toggles
2	Send booking confirmation notification	Medium	2	To Do	After successful booking
3	Send reminder before booking starts	Medium	3	To Do	1 hour before, configurable
4	Notify on booking cancellation	Low	2	To Do	When user or admin cancels
5	Notify on booking modification	Low	2	To Do	If time/spot changes
6	Push notifications for mobile app	Low	5	To Do	Integrate with Firebase/APNS
7	SMS integration	Low	4	To Do	Twilio or similar
8	In-app notification centre	Medium	4	To Do	Store recent notifications
9	Opt-out option	Medium	2	To Do	Respect user preferences
10	Notification history	Low	2	To Do	Log of sent notifications
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 14 					
As admin, I want to set dynamic pricing based on demand 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Define pricing rules engine	Medium	6	To Do	Configure rules (time, occupancy, events)
2	Integrate with real-time occupancy	Medium	4	To Do	Adjust prices based on current demand
3	UI for setting dynamic pricing parameters	Medium	4	To Do	Sliders for base price, surge multiplier
4	Historical data analysis	Low	5	To Do	Use past data to suggest optimal pricing
5	Price preview for users	Low	3	To Do	Show estimated price before booking
6	Cap maximum price	High	2	To Do	Prevent excessive surge pricing
7	A/B testing framework	Low	6	To Do	Test different pricing strategies
8	Report on revenue impact	Low	4	To Do	Compare dynamic vs static pricing
9					
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 15 					
As a user, I want to pay for parking online 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Integrate payment gateway (Stripe/PayPal)	High	8	To Do	Set up API keys, webhooks
2	Design checkout page	High	4	To Do	Secure payment form
3	Handle payment success/failure	High	3	To Do	Redirect and update booking status
4	Store payment method for future use	Medium	5	To Do	Tokenization, saved cards
5	Refund logic	Medium	4	To Do	Process refunds via gateway
6	Invoice generation	Low	3	To Do	PDF invoice after payment
7	Support multiple currencies	Low	4	To Do	Currency conversion
8	Tax calculation	Medium	3	To Do	Apply VAT/GST based on location
9	Payment history in user profile	Low	2	To Do	List of past transactions
10	Receipt email	Low	2	To Do	Send receipt after successful payment
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 16 					
As a user, I want to extend my booking duration					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Add "Extend" button on active booking	Medium	2	To Do	Only if spot is still available
2	Check availability for extended time	Medium	3	To Do	Ensure spot not booked by others
3	Calculate additional cost	Medium	2	To Do	Prorated based on rate
4	Process additional payment	High	4	To Do	Charge only the extra amount
5	Update booking end time	Medium	2	To Do	Modify database record
6	Send extension confirmation	Low	2	To Do	Email/SMS notification
7	Limit max extension duration	Medium	2	To Do	Configurable policy
8	Notify admin if extension conflicts	Low	3	To Do	Alert if spot was reserved for someone else
9					
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 17 					
As admin, I want to monitor live occupancy 					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Dashboard with live occupancy	Medium	4	To Do	Show total spots, occupied, free
2	Real-time updates via WebSockets	Medium	5	To Do	Push occupancy changes
3	Heatmap of occupancy	Low	4	To Do	Visual representation of crowded areas
4	Alerts for near full carparks	Low	3	To Do	Notify admin when >90% full
5	Historical occupancy trends	Low	4	To Do	Graphs for past days/weeks
6	Filter by zone/floor	Medium	2	To Do	Drill down to specific areas
7	Export occupancy snapshot	Low	2	To Do	CSV of current status
8	Integrate with sensor data (IoT)	Low	8	To Do	If hardware sensors exist
9					
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
				
User Story 18 					
As a user, I want to add multiple vehicles to my profile					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Design "My Vehicles" page	Medium	3	To Do	List of vehicles with add/edit/delete
2	Add vehicle form	Medium	2	To Do	License plate, make, model, color
3	Validate license plate format	Medium	2	To Do	Regex per country
4	Set default vehicle for bookings	Low	2	To Do	Pre select during booking
5	Store vehicle data in database	Medium	3	To Do	Separate table linked to user
6	Allow quick selection during booking	Medium	3	To Do	Dropdown of saved vehicles
7	Support for multiple plates per vehicle	Low	3	To Do	e.g., trailer plates
8	Vehicle photos	Low	4	To Do	Upload and store images
9					
10					
11					
12					
13					
14					
15					
16					
17					
18					
19					
20					
21					
22					
23					
24					
25					
User Story 19 					
As admin, I want to integrate with external calendar systems					
ID	Task Name	Priority	Expected Hours	Status	Details
1	Research calendar APIs (Google, Outlook)	Low	3	To Do	Understand OAuth and endpoints
2	Implement OAuth2 flow for calendar access	Low	5	To Do	User consent and token storage
3	Sync bookings to external calendar	Low	4	To Do	Create events when booking made
4	Sync changes (update/delete)	Low	3	To Do	Keep calendar in sync
5	Allow users to select which calendar	Low	2	To Do	Dropdown in settings
6	Two way sync? (optional)	Low	6	To Do	Import external events as bookings?
7	Error handling and retries	Low	3	To Do	Handle API failures gracefully
8	Admin dashboard for connected accounts	Low	2	To Do	View and revoke connections