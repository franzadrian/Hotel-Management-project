-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 07, 2025 at 12:52 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hrsm`
--

-- --------------------------------------------------------

--
-- Table structure for table `activity`
--

CREATE TABLE `activity` (
  `ACTIVITY_ID` int(11) NOT NULL,
  `MESSAGE` text NOT NULL,
  `ACTIVITY_TYPE` varchar(50) NOT NULL,
  `ICON` varchar(50) NOT NULL,
  `RELATED_ID` int(11) DEFAULT NULL,
  `CREATED_AT` datetime NOT NULL,
  `RELATED_URL` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `activity`
--

INSERT INTO `activity` (`ACTIVITY_ID`, `MESSAGE`, `ACTIVITY_TYPE`, `ICON`, `RELATED_ID`, `CREATED_AT`, `RELATED_URL`) VALUES
(1, 'New booking created for Deluxe Room 405 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 1, '2025-05-04 18:42:24', '/Admin/ReservationManagement'),
(2, 'Booking #LC2025-7799 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 1, '2025-05-04 18:43:27', '/Admin/ReservationManagement'),
(3, 'Booking #LC2025-7799 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 1, '2025-05-04 18:43:40', '/Admin/ReservationManagement'),
(4, 'New 1-star feedback for Deluxe room from Bonch Eturma: \"bati\"', 'feedback', 'far fa-star', 1, '2025-05-04 18:59:18', '/Admin/FeedbackManagement'),
(5, 'Admin responded to feedback from Bonch Eturma: \"imong nawng\"', 'feedback', 'fas fa-reply', 1, '2025-05-04 19:02:27', '/Admin/FeedbackManagement'),
(6, 'Admin responded to feedback from Bonch Eturma: \"imong nawng haha\"', 'feedback', 'fas fa-reply', 1, '2025-05-04 19:03:02', '/Admin/FeedbackManagement'),
(7, 'Admin responded to feedback from Bonch Eturma: \"imong nawng\"', 'feedback', 'fas fa-reply', 1, '2025-05-04 19:03:09', '/Admin/FeedbackManagement'),
(8, 'New Maintenance (Electrical) request from Room 405 by Bonch Eturma', 'service', 'fas fa-tools', 1, '2025-05-04 19:04:31', '/Admin/ServicesManagement'),
(9, 'New Housekeeping request from Room 405 by Bonch Eturma', 'service', 'fas fa-broom', 2, '2025-05-04 19:04:53', '/Admin/ServicesManagement'),
(10, 'Maintenance (Electrical) request for Room 405 status changed from Pending to In Progress', 'service', 'fas fa-tools', 1, '2025-05-04 19:05:11', '/Admin/ServicesManagement'),
(11, 'Housekeeping request for Room 405 status changed from Pending to In Progress', 'service', 'fas fa-broom', 2, '2025-05-04 19:05:17', '/Admin/ServicesManagement'),
(12, 'Housekeeping request for Room 405 status changed from In Progress to Completed', 'service', 'fas fa-broom', 2, '2025-05-04 19:05:20', '/Admin/ServicesManagement'),
(13, 'Maintenance (Electrical) request for Room 405 status changed from In Progress to Completed', 'service', 'fas fa-tools', 1, '2025-05-04 19:05:23', '/Admin/ServicesManagement'),
(14, 'Events page title updated to \'April Eventsss\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:46:52', '/Admin/EventManagement'),
(15, 'Events page title updated to \'April Eventsss\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:46:57', '/Admin/EventManagement'),
(16, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:47:58', '/Admin/EventManagement'),
(17, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:48:15', '/Admin/EventManagement'),
(18, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:48:34', '/Admin/EventManagement'),
(19, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:54:18', '/Admin/EventManagement'),
(20, 'Events page title updated to \'April Event\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:54:33', '/Admin/EventManagement'),
(21, 'Events page title updated to \'April Eventsss\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:55:09', '/Admin/EventManagement'),
(22, 'Events page title updated to \'April Event\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:55:23', '/Admin/EventManagement'),
(23, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 19:56:06', '/Admin/EventManagement'),
(24, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:01:21', '/Admin/EventManagement'),
(25, 'Event \'FREE DRINKS\' updated', 'event', 'fas fa-edit', 2, '2025-05-04 20:01:53', '/Admin/EventManagement'),
(26, 'Event \'FREE DRINKS\' updated', 'event', 'fas fa-edit', 2, '2025-05-04 20:02:12', '/Admin/EventManagement'),
(27, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 20:02:30', '/Admin/EventManagement'),
(28, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 20:02:39', '/Admin/EventManagement'),
(29, 'Event \'FINE DINEee\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 20:02:52', '/Admin/EventManagement'),
(30, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 20:03:06', '/Admin/EventManagement'),
(31, 'New event \'wa\' created', 'event', 'fas fa-calendar', 4, '2025-05-04 20:03:20', '/Admin/EventManagement'),
(32, 'Event \'wa\' deleted', 'event', 'fas fa-trash', 4, '2025-05-04 20:03:41', '/Admin/EventManagement'),
(33, 'Event \'wa\' deleted', 'event', 'fas fa-trash', 4, '2025-05-04 20:03:46', '/Admin/EventManagement'),
(34, 'Events page title updated to \'April Eventssss\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:15:00', '/Admin/EventManagement'),
(35, 'Events page title updated to \'April Event\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:15:31', '/Admin/EventManagement'),
(36, 'Event \'wa\' deleted', 'event', 'fas fa-trash', 4, '2025-05-04 20:15:56', '/Admin/EventManagement'),
(37, 'Events page title updated to \'March\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:16:03', '/Admin/EventManagement'),
(38, 'Events page title updated to \'March\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:17:15', '/Admin/EventManagement'),
(39, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:17:22', '/Admin/EventManagement'),
(40, 'Events page title updated to \'Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:18:29', '/Admin/EventManagement'),
(41, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:20:44', '/Admin/EventManagement'),
(42, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:23:15', '/Admin/EventManagement'),
(43, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:23:56', '/Admin/EventManagement'),
(44, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:29:42', '/Admin/EventManagement'),
(45, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:30:27', '/Admin/EventManagement'),
(46, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 20:30:48', '/Admin/EventManagement'),
(47, 'New event \'FREE KAON\' created', 'event', 'fas fa-calendar', 5, '2025-05-04 20:37:59', '/Admin/EventManagement'),
(48, 'Event \'FREE KAON\' deleted', 'event', 'fas fa-trash', 5, '2025-05-04 20:38:53', '/Admin/EventManagement'),
(49, 'Booking #LC2025-7799 status changed from CheckedIn to CheckedOut for guest Bonch Eturma', 'booking', 'fas fa-door-closed', 1, '2025-05-04 20:58:13', '/Admin/ReservationManagement'),
(50, 'New booking created for Suite Room 501 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 2, '2025-05-04 21:02:16', '/Admin/ReservationManagement'),
(51, 'Booking #LC2025-6170 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 2, '2025-05-04 21:02:46', '/Admin/ReservationManagement'),
(52, 'Booking #LC2025-6170 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 2, '2025-05-04 21:02:56', '/Admin/ReservationManagement'),
(53, 'Booking #LC2025-6170 status changed from CheckedIn to CheckedOut for guest Bonch Eturma', 'booking', 'fas fa-door-closed', 2, '2025-05-04 21:03:01', '/Admin/ReservationManagement'),
(54, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 22:13:57', '/Admin/EventManagement'),
(55, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 22:14:14', '/Admin/EventManagement'),
(56, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 22:17:05', '/Admin/EventManagement'),
(57, 'Event \'FREE DRINKS\' updated', 'event', 'fas fa-edit', 2, '2025-05-04 22:17:17', '/Admin/EventManagement'),
(58, 'Event \'FREE DRINKS\' updated', 'event', 'fas fa-edit', 2, '2025-05-04 22:17:27', '/Admin/EventManagement'),
(59, 'Event \'FINE DINE\' updated', 'event', 'fas fa-edit', 1, '2025-05-04 22:25:32', '/Admin/EventManagement'),
(60, 'Event \'FREE DRINKS\' updated', 'event', 'fas fa-edit', 2, '2025-05-04 22:25:42', '/Admin/EventManagement'),
(61, 'Event \'POOL PARTY\' updated', 'event', 'fas fa-edit', 3, '2025-05-04 22:25:54', '/Admin/EventManagement'),
(62, 'New event \'FREE KAON\' created', 'event', 'fas fa-calendar', 6, '2025-05-04 22:26:46', '/Admin/EventManagement'),
(63, 'Events page title updated to \'March Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 22:26:59', '/Admin/EventManagement'),
(64, 'Event \'FREE KAON\' deleted', 'event', 'fas fa-trash', 6, '2025-05-04 22:27:56', '/Admin/EventManagement'),
(65, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-04 22:28:02', '/Admin/EventManagement'),
(66, 'New booking created for Suite Room 506 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 3, '2025-05-04 22:29:22', '/Admin/ReservationManagement'),
(67, 'New booking created for Standard Room 301 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 4, '2025-05-05 00:00:45', '/Admin/ReservationManagement'),
(68, 'Events page title updated to \'April Events\'', 'event', 'fas fa-heading', NULL, '2025-05-07 00:45:34', '/Admin/EventManagement'),
(69, 'Booking #LC2025-1368 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 4, '2025-05-07 02:22:22', '/Admin/ReservationManagement'),
(70, 'Booking #LC2025-1368 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 4, '2025-05-07 02:22:29', '/Admin/ReservationManagement'),
(71, 'Booking #LC2025-1368 status changed from CheckedIn to CheckedOut for guest Bonch Eturma', 'booking', 'fas fa-door-closed', 4, '2025-05-07 02:23:15', '/Admin/ReservationManagement'),
(72, 'Booking #LC2025-3251 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 3, '2025-05-07 02:23:18', '/Admin/ReservationManagement'),
(73, 'Booking #LC2025-3251 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 3, '2025-05-07 02:23:21', '/Admin/ReservationManagement'),
(74, 'Booking #LC2025-3251 status changed from CheckedIn to CheckedOut for guest Bonch Eturma', 'booking', 'fas fa-door-closed', 3, '2025-05-07 02:23:24', '/Admin/ReservationManagement'),
(75, 'New booking created for Deluxe Room 402 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 21, '2025-05-07 02:24:20', '/Admin/ReservationManagement'),
(76, 'Booking #LC2025-9354 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 21, '2025-05-07 02:24:51', '/Admin/ReservationManagement'),
(77, 'Booking #LC2025-9354 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 21, '2025-05-07 02:25:03', '/Admin/ReservationManagement'),
(78, 'Booking #LC2025-9354 status changed from CheckedIn to CheckedOut for guest Bonch Eturma', 'booking', 'fas fa-door-closed', 21, '2025-05-07 03:11:14', '/Admin/ReservationManagement'),
(79, 'New booking created for Standard Room 302 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 46, '2025-05-07 03:11:48', '/Admin/ReservationManagement'),
(80, 'Booking #LC2025-3719 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 46, '2025-05-07 03:13:03', '/Admin/ReservationManagement'),
(81, 'Booking #LC2025-3719 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 46, '2025-05-07 03:13:28', '/Admin/ReservationManagement'),
(82, 'Booking #LC2025-3719 status changed from CheckedIn to CheckedOut for guest Bonch Eturma', 'booking', 'fas fa-door-closed', 46, '2025-05-07 03:13:51', '/Admin/ReservationManagement'),
(83, 'New booking created for Standard Room 301 by Bonch Eturma', 'booking', 'fas fa-calendar-check', 47, '2025-05-07 04:09:45', '/Admin/ReservationManagement'),
(84, 'New 5-star feedback for Deluxe room from Bonch Eturma: \"Nice\"', 'feedback', 'fas fa-star', 2, '2025-05-07 04:39:52', '/Admin/FeedbackManagement'),
(85, 'Booking #LC2025-8491 status changed from Pending to Confirmed for guest Bonch Eturma', 'booking', 'fas fa-check-circle', 47, '2025-05-07 04:51:13', '/Admin/ReservationManagement'),
(86, 'Booking #LC2025-8491 status changed from Confirmed to CheckedIn for guest Bonch Eturma', 'booking', 'fas fa-door-open', 47, '2025-05-07 04:51:17', '/Admin/ReservationManagement'),
(87, 'New maintenance request created for room 301 by Bonch Eturma', 'service', 'fas fa-tools', 3, '2025-05-07 05:54:44', '/Admin/ServiceRequestManagement'),
(88, 'New housekeeping request created for room 301 by Bonch Eturma', 'service', 'fas fa-broom', 4, '2025-05-07 05:54:53', '/Admin/ServiceRequestManagement'),
(89, 'Maintenance (Electrical) request for Room 301 status changed from Pending to In Progress', 'service', 'fas fa-tools', 3, '2025-05-07 05:57:45', '/Admin/ServicesManagement'),
(90, 'Room 301 (standard) status changed from available to maintenance by Jamie Roberts', 'room', 'fas fa-tools', 1, '2025-05-07 05:57:55', '/Admin/RoomManagement'),
(91, 'Housekeeping request for Room 301 status changed from Pending to In Progress', 'service', 'fas fa-broom', 4, '2025-05-07 05:58:10', '/Admin/ServicesManagement'),
(92, 'Admin responded to feedback from Bonch Eturma: \"Thank you <3!\"', 'feedback', 'fas fa-reply', 2, '2025-05-07 06:30:14', '/Admin/FeedbackManagement'),
(93, 'Maintenance (Electrical) request for Room 301 status changed from In Progress to Completed', 'service', 'fas fa-tools', 3, '2025-05-07 06:30:49', '/Admin/ServicesManagement'),
(94, 'Housekeeping request for Room 301 status changed from In Progress to Completed', 'service', 'fas fa-broom', 4, '2025-05-07 06:30:57', '/Admin/ServicesManagement'),
(95, 'New maintenance request created for room 301 by Bonch Eturma', 'service', 'fas fa-tools', 5, '2025-05-07 06:32:46', '/Admin/ServiceRequestManagement'),
(96, 'New housekeeping request created for room 301 by Bonch Eturma', 'service', 'fas fa-broom', 6, '2025-05-07 06:32:53', '/Admin/ServiceRequestManagement'),
(97, 'Maintenance (Plumbing) request for Room 301 status changed from Pending to In Progress', 'service', 'fas fa-tools', 5, '2025-05-07 06:34:47', '/Admin/ServicesManagement'),
(98, 'Maintenance (Plumbing) request for Room 301 status changed from In Progress to Completed', 'service', 'fas fa-tools', 5, '2025-05-07 06:34:49', '/Admin/ServicesManagement'),
(99, 'Housekeeping request for Room 301 status changed from Pending to In Progress', 'service', 'fas fa-broom', 6, '2025-05-07 06:37:12', '/Admin/ServicesManagement'),
(100, 'Housekeeping request for Room 301 status changed from In Progress to Completed', 'service', 'fas fa-broom', 6, '2025-05-07 06:37:14', '/Admin/ServicesManagement'),
(101, 'New user registered: Jose Rizal (jose.rizal@example.com)', 'user', 'fas fa-user-plus', 14, '2025-05-07 06:47:22', '/Admin/UserManagement');

-- --------------------------------------------------------

--
-- Table structure for table `booking`
--

CREATE TABLE `booking` (
  `BOOKING_ID` int(11) NOT NULL,
  `RESERVATION_NUMBER` longtext NOT NULL,
  `USER_ID` int(11) NOT NULL,
  `ROOM_NUMBER` longtext NOT NULL,
  `ROOM_TYPE` longtext NOT NULL,
  `ROOM_RATE` decimal(65,30) NOT NULL,
  `CHECK_IN_DATE` datetime(6) NOT NULL,
  `CHECK_OUT_DATE` datetime(6) NOT NULL,
  `BOOKING_DATE` datetime(6) NOT NULL,
  `BOOKING_TYPE` longtext NOT NULL,
  `NUM_ADULTS` int(11) NOT NULL,
  `NUM_CHILDREN` int(11) NOT NULL,
  `EXTRA_BEDS` int(11) NOT NULL,
  `EXTRA_ROOMS` int(11) NOT NULL,
  `SPA_SERVICE` tinyint(1) NOT NULL,
  `ROOM_SERVICE` tinyint(1) NOT NULL,
  `AIRPORT_TRANSFER` tinyint(1) NOT NULL,
  `SPECIAL_REQUESTS` longtext DEFAULT NULL,
  `TOTAL_AMOUNT` decimal(65,30) NOT NULL,
  `STATUS` longtext NOT NULL,
  `PAYMENT_STATUS` longtext NOT NULL,
  `PAYMENT_METHOD` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `booking`
--

INSERT INTO `booking` (`BOOKING_ID`, `RESERVATION_NUMBER`, `USER_ID`, `ROOM_NUMBER`, `ROOM_TYPE`, `ROOM_RATE`, `CHECK_IN_DATE`, `CHECK_OUT_DATE`, `BOOKING_DATE`, `BOOKING_TYPE`, `NUM_ADULTS`, `NUM_CHILDREN`, `EXTRA_BEDS`, `EXTRA_ROOMS`, `SPA_SERVICE`, `ROOM_SERVICE`, `AIRPORT_TRANSFER`, `SPECIAL_REQUESTS`, `TOTAL_AMOUNT`, `STATUS`, `PAYMENT_STATUS`, `PAYMENT_METHOD`) VALUES
(1, 'LC2025-7799', 1, '405', 'Deluxe', 3588.000000000000000000000000000000, '2025-05-04 00:00:00.000000', '2025-05-05 00:00:00.000000', '2025-05-04 18:42:24.513983', 'Immediate Booking', 2, 3, 1, 0, 1, 1, 1, '', 7588.000000000000000000000000000000, 'CheckedOut', 'Paid', 'GCash'),
(2, 'LC2025-6170', 1, '501', 'Suite', 15000.000000000000000000000000000000, '2025-05-04 00:00:00.000000', '2025-05-05 00:00:00.000000', '2025-05-04 21:02:15.663038', 'Immediate Booking', 2, 3, 1, 0, 1, 1, 1, '', 19000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Pay at Hotel'),
(3, 'LC2025-3251', 1, '506', 'Suite', 15000.000000000000000000000000000000, '2025-05-04 00:00:00.000000', '2025-05-05 00:00:00.000000', '2025-05-04 22:29:21.803079', 'Immediate Booking', 2, 3, 1, 0, 1, 1, 1, '', 19000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Pay at Hotel'),
(4, 'LC2025-1368', 1, '301', 'Standard', 1806.000000000000000000000000000000, '2025-05-07 00:00:00.000000', '2025-05-09 00:00:00.000000', '2025-05-05 00:00:44.106987', 'Advanced Booking', 2, 3, 1, 1, 1, 1, 1, '', 12224.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Pay at Hotel'),
(5, 'RES-2024-001', 1, '101', 'Standard', 150.000000000000000000000000000000, '2024-01-15 00:00:00.000000', '2024-01-18 00:00:00.000000', '2024-01-10 00:00:00.000000', 'Online', 2, 0, 0, 0, 0, 1, 0, 'Late check-in', 450.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(6, 'RES-2024-002', 1, '201', 'Deluxe', 200.000000000000000000000000000000, '2024-02-10 00:00:00.000000', '2024-02-15 00:00:00.000000', '2024-02-01 00:00:00.000000', 'Online', 2, 1, 1, 0, 1, 1, 0, 'Extra towels', 1000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(7, 'RES-2024-003', 1, '301', 'Suite', 350.000000000000000000000000000000, '2024-03-20 00:00:00.000000', '2024-03-25 00:00:00.000000', '2024-03-10 00:00:00.000000', 'Phone', 2, 2, 0, 0, 1, 1, 1, 'Airport pickup at 2PM', 1750.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(8, 'RES-2024-004', 1, '102', 'Standard', 150.000000000000000000000000000000, '2024-04-05 00:00:00.000000', '2024-04-08 00:00:00.000000', '2024-04-01 00:00:00.000000', 'Online', 1, 0, 0, 0, 0, 0, 0, NULL, 450.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(9, 'RES-2024-005', 1, '202', 'Deluxe', 200.000000000000000000000000000000, '2024-05-15 00:00:00.000000', '2024-05-20 00:00:00.000000', '2024-05-01 00:00:00.000000', 'Phone', 2, 0, 0, 0, 1, 0, 0, 'Need early check-in', 1000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(10, 'RES-2024-006', 1, '302', 'Suite', 350.000000000000000000000000000000, '2024-06-22 00:00:00.000000', '2024-06-25 00:00:00.000000', '2024-06-10 00:00:00.000000', 'Online', 2, 0, 0, 0, 1, 1, 0, 'Anniversary celebration', 1050.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(11, 'RES-2024-007', 1, '103', 'Standard', 150.000000000000000000000000000000, '2024-07-10 00:00:00.000000', '2024-07-15 00:00:00.000000', '2024-07-01 00:00:00.000000', 'Online', 2, 1, 1, 0, 0, 1, 0, NULL, 750.000000000000000000000000000000, 'CheckedOut', 'Paid', 'PayPal'),
(12, 'RES-2024-008', 1, '203', 'Deluxe', 200.000000000000000000000000000000, '2024-08-20 00:00:00.000000', '2024-08-25 00:00:00.000000', '2024-08-15 00:00:00.000000', 'Phone', 2, 2, 0, 0, 0, 1, 1, 'Need baby crib', 1000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(13, 'RES-2024-009', 1, '303', 'Suite', 350.000000000000000000000000000000, '2024-09-15 00:00:00.000000', '2024-09-20 00:00:00.000000', '2024-09-01 00:00:00.000000', 'Online', 2, 0, 0, 0, 1, 1, 0, 'Honeymoon setup', 1750.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(14, 'RES-2024-010', 1, '104', 'Standard', 150.000000000000000000000000000000, '2024-10-10 00:00:00.000000', '2024-10-15 00:00:00.000000', '2024-10-01 00:00:00.000000', 'Online', 1, 0, 0, 0, 0, 0, 0, NULL, 750.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(15, 'RES-2024-011', 1, '204', 'Deluxe', 200.000000000000000000000000000000, '2024-11-20 00:00:00.000000', '2024-11-25 00:00:00.000000', '2024-11-10 00:00:00.000000', 'Phone', 2, 1, 1, 0, 0, 1, 0, 'Late checkout requested', 1000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(16, 'RES-2024-012', 1, '304', 'Suite', 350.000000000000000000000000000000, '2024-12-25 00:00:00.000000', '2024-12-30 00:00:00.000000', '2024-12-15 00:00:00.000000', 'Online', 4, 0, 2, 0, 1, 1, 1, 'New Year celebration', 1750.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(17, 'RES-2025-001', 1, '105', 'Standard', 160.000000000000000000000000000000, '2025-01-10 00:00:00.000000', '2025-01-14 00:00:00.000000', '2025-01-05 00:00:00.000000', 'Online', 2, 0, 0, 0, 0, 1, 0, NULL, 640.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(18, 'RES-2025-002', 1, '205', 'Deluxe', 220.000000000000000000000000000000, '2025-02-14 00:00:00.000000', '2025-02-16 00:00:00.000000', '2025-02-01 00:00:00.000000', 'Online', 2, 0, 0, 0, 1, 1, 0, 'Valentine\'s Day package', 440.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(19, 'RES-2025-003', 1, '305', 'Suite', 380.000000000000000000000000000000, '2025-03-20 00:00:00.000000', '2025-03-25 00:00:00.000000', '2025-03-10 00:00:00.000000', 'Phone', 2, 2, 0, 0, 1, 1, 1, 'Spring break vacation', 1900.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(20, 'RES-2025-004', 1, '106', 'Standard', 160.000000000000000000000000000000, '2025-04-10 00:00:00.000000', '2025-04-15 00:00:00.000000', '2025-04-01 00:00:00.000000', 'Online', 2, 0, 0, 0, 0, 0, 0, NULL, 800.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(21, 'LC2025-9354', 1, '402', 'Deluxe', 3588.000000000000000000000000000000, '2025-05-09 00:00:00.000000', '2025-05-11 00:00:00.000000', '2025-05-07 02:24:20.693954', 'Advanced Booking', 2, 3, 1, 1, 1, 1, 1, '', 19352.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Pay at Hotel'),
(22, 'RES-2024-001', 1, '434', 'Deluxe', 3588.000000000000000000000000000000, '2024-01-01 00:00:00.000000', '2024-01-06 00:00:00.000000', '2023-12-25 00:00:00.000000', 'Online', 1, 0, 0, 1, 0, 1, 1, 'Generated booking record', 37880.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(23, 'RES-2024-002', 1, '462', 'Suite', 15000.000000000000000000000000000000, '2024-01-16 00:00:00.000000', '2024-01-19 00:00:00.000000', '2024-01-09 00:00:00.000000', 'Walk-in', 1, 1, 1, 0, 1, 0, 1, 'Generated booking record', 50200.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Bank Transfer'),
(24, 'RES-2024-003', 1, '383', 'Suite', 15000.000000000000000000000000000000, '2024-02-04 00:00:00.000000', '2024-02-06 00:00:00.000000', '2024-01-23 00:00:00.000000', 'Walk-in', 1, 0, 1, 0, 1, 1, 0, 'Generated booking record', 33800.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(25, 'RES-2024-004', 1, '579', 'Suite', 15000.000000000000000000000000000000, '2024-02-21 00:00:00.000000', '2024-02-26 00:00:00.000000', '2024-02-13 00:00:00.000000', 'Walk-in', 2, 0, 0, 0, 0, 1, 0, 'Generated booking record', 75800.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(26, 'RES-2024-005', 1, '226', 'Deluxe', 3588.000000000000000000000000000000, '2024-03-12 00:00:00.000000', '2024-03-14 00:00:00.000000', '2024-02-28 00:00:00.000000', 'Phone', 3, 2, 1, 0, 0, 0, 0, 'Generated booking record', 9176.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(27, 'RES-2024-006', 1, '576', 'Suite', 15000.000000000000000000000000000000, '2024-03-26 00:00:00.000000', '2024-03-30 00:00:00.000000', '2024-03-15 00:00:00.000000', 'Walk-in', 2, 0, 0, 1, 0, 1, 0, 'Generated booking record', 120800.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(28, 'RES-2024-007', 1, '145', 'Suite', 15000.000000000000000000000000000000, '2024-04-16 00:00:00.000000', '2024-04-19 00:00:00.000000', '2024-04-02 00:00:00.000000', 'Phone', 3, 2, 1, 0, 0, 0, 0, 'Generated booking record', 51200.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(29, 'RES-2024-008', 1, '426', 'Standard', 1806.000000000000000000000000000000, '2024-05-06 00:00:00.000000', '2024-05-08 00:00:00.000000', '2024-04-28 00:00:00.000000', 'Walk-in', 2, 1, 0, 0, 1, 0, 1, 'Generated booking record', 5800.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Cash'),
(30, 'RES-2024-009', 1, '512', 'Deluxe', 3588.000000000000000000000000000000, '2024-06-01 00:00:00.000000', '2024-06-03 00:00:00.000000', '2024-05-22 00:00:00.000000', 'Online', 2, 0, 1, 0, 0, 0, 1, 'Generated booking record', 9176.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(31, 'RES-2024-010', 1, '333', 'Suite', 15000.000000000000000000000000000000, '2024-06-21 00:00:00.000000', '2024-06-24 00:00:00.000000', '2024-06-10 00:00:00.000000', 'Walk-in', 1, 1, 0, 0, 1, 1, 0, 'Generated booking record', 50200.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Bank Transfer'),
(32, 'RES-2024-011', 1, '401', 'Standard', 1806.000000000000000000000000000000, '2024-07-03 00:00:00.000000', '2024-07-05 00:00:00.000000', '2024-06-25 00:00:00.000000', 'Online', 2, 2, 0, 1, 1, 0, 0, 'Generated booking record', 11432.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(33, 'RES-2024-012', 1, '288', 'Deluxe', 3588.000000000000000000000000000000, '2024-07-16 00:00:00.000000', '2024-07-21 00:00:00.000000', '2024-07-04 00:00:00.000000', 'Walk-in', 1, 0, 0, 0, 1, 1, 1, 'Generated booking record', 38880.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Cash'),
(34, 'RES-2024-013', 1, '199', 'Suite', 15000.000000000000000000000000000000, '2024-08-01 00:00:00.000000', '2024-08-05 00:00:00.000000', '2024-07-24 00:00:00.000000', 'Phone', 2, 0, 1, 0, 0, 0, 1, 'Generated booking record', 63800.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(35, 'RES-2024-014', 1, '314', 'Suite', 15000.000000000000000000000000000000, '2024-08-22 00:00:00.000000', '2024-08-26 00:00:00.000000', '2024-08-15 00:00:00.000000', 'Online', 1, 1, 0, 1, 0, 1, 0, 'Generated booking record', 90400.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(36, 'RES-2024-015', 1, '105', 'Standard', 1806.000000000000000000000000000000, '2024-09-10 00:00:00.000000', '2024-09-12 00:00:00.000000', '2024-09-01 00:00:00.000000', 'Walk-in', 2, 1, 1, 0, 0, 0, 1, 'Generated booking record', 7200.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Bank Transfer'),
(37, 'RES-2024-016', 1, '489', 'Deluxe', 3588.000000000000000000000000000000, '2024-10-04 00:00:00.000000', '2024-10-09 00:00:00.000000', '2024-09-26 00:00:00.000000', 'Online', 1, 0, 0, 0, 1, 0, 0, 'Generated booking record', 19040.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(38, 'RES-2025-001', 1, '217', 'Suite', 15000.000000000000000000000000000000, '2025-01-03 00:00:00.000000', '2025-01-07 00:00:00.000000', '2024-12-22 00:00:00.000000', 'Walk-in', 2, 2, 0, 1, 1, 1, 0, 'Generated booking record', 100400.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(39, 'RES-2025-002', 1, '382', 'Deluxe', 3588.000000000000000000000000000000, '2025-01-20 00:00:00.000000', '2025-01-22 00:00:00.000000', '2025-01-10 00:00:00.000000', 'Online', 1, 0, 1, 0, 0, 0, 0, 'Generated booking record', 9176.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Cash'),
(40, 'RES-2025-003', 1, '305', 'Suite', 15000.000000000000000000000000000000, '2025-03-20 00:00:00.000000', '2025-03-25 00:00:00.000000', '2025-03-10 00:00:00.000000', 'Phone', 2, 2, 0, 0, 1, 1, 1, 'Spring break vacation', 88000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(41, 'RES-2025-004', 1, '148', 'Standard', 1806.000000000000000000000000000000, '2025-04-05 00:00:00.000000', '2025-04-07 00:00:00.000000', '2025-03-25 00:00:00.000000', 'Walk-in', 2, 0, 1, 0, 0, 1, 0, 'Generated booking record', 7600.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(42, 'RES-2025-005', 1, '498', 'Deluxe', 3588.000000000000000000000000000000, '2025-04-22 00:00:00.000000', '2025-04-25 00:00:00.000000', '2025-04-10 00:00:00.000000', 'Phone', 3, 1, 0, 1, 1, 1, 1, 'Family getaway', 50200.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Debit Card'),
(43, 'RES-2024-017', 1, '187', 'Suite', 15000.000000000000000000000000000000, '2024-10-22 00:00:00.000000', '2024-10-25 00:00:00.000000', '2024-10-10 00:00:00.000000', 'Online', 2, 1, 1, 0, 0, 1, 0, 'Anniversary trip', 50200.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Bank Transfer'),
(44, 'RES-2024-018', 1, '120', 'Standard', 1806.000000000000000000000000000000, '2024-11-05 00:00:00.000000', '2024-11-08 00:00:00.000000', '2024-10-30 00:00:00.000000', 'Phone', 1, 0, 0, 0, 1, 0, 1, 'Generated booking record', 10000.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Cash'),
(45, 'RES-2024-019', 1, '415', 'Deluxe', 3588.000000000000000000000000000000, '2024-12-15 00:00:00.000000', '2024-12-20 00:00:00.000000', '2024-12-01 00:00:00.000000', 'Online', 3, 2, 1, 1, 1, 1, 1, 'Holiday vacation', 60576.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Credit Card'),
(46, 'LC2025-3719', 1, '302', 'Standard', 1806.000000000000000000000000000000, '2025-05-07 00:00:00.000000', '2025-05-08 00:00:00.000000', '2025-05-07 03:11:47.873269', 'Immediate Booking', 2, 3, 0, 0, 0, 1, 0, '', 2606.000000000000000000000000000000, 'CheckedOut', 'Paid', 'Pay at Hotel'),
(47, 'LC2025-8491', 1, '301', 'Standard', 1806.000000000000000000000000000000, '2025-05-07 00:00:00.000000', '2025-05-08 00:00:00.000000', '2025-05-07 04:09:45.207463', 'Immediate Booking', 2, 1, 0, 0, 0, 0, 0, '', 1806.000000000000000000000000000000, 'CheckedIn', 'Paid', 'Pay at Hotel');

-- --------------------------------------------------------

--
-- Table structure for table `event`
--

CREATE TABLE `event` (
  `EVENT_ID` int(11) NOT NULL,
  `TITLE` varchar(100) NOT NULL,
  `LOCATION` varchar(100) NOT NULL,
  `DESCRIPTION` varchar(100) NOT NULL,
  `EVENT_DATE` varchar(100) NOT NULL,
  `PARTICIPANTS` varchar(100) NOT NULL,
  `NOTE` varchar(255) DEFAULT NULL,
  `IMAGE_PATH` varchar(255) NOT NULL,
  `DISPLAY_ORDER` int(11) NOT NULL,
  `IS_ACTIVE` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `event`
--

INSERT INTO `event` (`EVENT_ID`, `TITLE`, `LOCATION`, `DESCRIPTION`, `EVENT_DATE`, `PARTICIPANTS`, `NOTE`, `IMAGE_PATH`, `DISPLAY_ORDER`, `IS_ACTIVE`) VALUES
(1, 'FINE DINE', 'Dining Area', 'Fine Dine', '2025-05-10', 'Everyone in Hotel', 'P.S. You Can Bring Your Love One\'s', '/images/evepic1.png', 1, 1),
(2, 'FREE DRINKS', 'Bar Area', 'Free Drinks', '2025-05-15', 'Everyone in Hotel', 'P.S. You Can Bring Your Love One\'s', '/images/evepic2.png', 2, 1),
(3, 'POOL PARTY', 'Rooftop Pool', 'Pool Party', '2025-05-25', 'Everyone in Hotel', 'P.S. You Can Bring Your Love One\'s', '/images/evepic3.png', 3, 1);

-- --------------------------------------------------------

--
-- Table structure for table `feedback`
--

CREATE TABLE `feedback` (
  `FEEDBACK_ID` int(11) NOT NULL,
  `USER_ID` int(11) NOT NULL,
  `ROOM_NUMBER` varchar(10) NOT NULL,
  `ROOM_TYPE` varchar(50) NOT NULL,
  `RATING` int(11) NOT NULL,
  `TITLE` varchar(100) NOT NULL,
  `COMMENT` text NOT NULL,
  `SUBMIT_DATE` datetime NOT NULL,
  `STAY_TYPE` varchar(20) NOT NULL,
  `ADMIN_RESPONSE` text DEFAULT NULL,
  `RESPONSE_DATE` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `feedback`
--

INSERT INTO `feedback` (`FEEDBACK_ID`, `USER_ID`, `ROOM_NUMBER`, `ROOM_TYPE`, `RATING`, `TITLE`, `COMMENT`, `SUBMIT_DATE`, `STAY_TYPE`, `ADMIN_RESPONSE`, `RESPONSE_DATE`) VALUES
(1, 1, '', 'Deluxe', 1, '', 'bati', '2025-05-04 18:59:17', 'CheckedIn', 'imong nawng', '2025-05-04 19:03:09'),
(2, 1, '', 'Deluxe', 5, '', 'Nice', '2025-05-07 04:39:50', 'CheckedOut', 'Thank you <3!', '2025-05-07 06:30:14');

-- --------------------------------------------------------

--
-- Table structure for table `notification`
--

CREATE TABLE `notification` (
  `NOTIFICATION_ID` int(11) NOT NULL,
  `USER_ID` int(11) NOT NULL,
  `MESSAGE` text NOT NULL,
  `TYPE` varchar(50) NOT NULL,
  `ICON` varchar(50) NOT NULL,
  `RELATED_ID` int(11) DEFAULT NULL,
  `CREATED_AT` datetime NOT NULL,
  `IS_READ` tinyint(1) NOT NULL DEFAULT 0,
  `RELATED_URL` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `notification`
--

INSERT INTO `notification` (`NOTIFICATION_ID`, `USER_ID`, `MESSAGE`, `TYPE`, `ICON`, `RELATED_ID`, `CREATED_AT`, `IS_READ`, `RELATED_URL`) VALUES
(1, 1, 'Your booking for Standard Room 301 has been created successfully', 'booking', 'fas fa-calendar-check', 47, '2025-05-07 04:09:45', 1, '/Account/UserInfo'),
(2, 1, 'Thank you for submitting your 5-star feedback for Deluxe room', 'feedback', 'fas fa-star', 2, '2025-05-07 04:39:52', 1, '/Account/UserInfo'),
(3, 1, 'Your booking #LC2025-8491 for Standard has been confirmed', 'booking', 'fas fa-check-circle', 47, '2025-05-07 04:51:13', 1, '/Account/UserInfo'),
(4, 1, 'Welcome! You have been checked in to your Standard room', 'booking', 'fas fa-door-open', 47, '2025-05-07 04:51:17', 1, '/Account/UserInfo'),
(5, 1, 'Your maintenance request for room 301 has been submitted successfully', 'service', 'fas fa-tools', 3, '2025-05-07 05:54:44', 1, '/Sidebar/ServiceRequest'),
(6, 1, 'Your housekeeping request for room 301 has been submitted successfully', 'service', 'fas fa-broom', 4, '2025-05-07 05:54:54', 1, '/Sidebar/ServiceRequest'),
(7, 1, 'We\'ve responded to your feedback about Deluxe room', 'feedback', 'fas fa-reply', 2, '2025-05-07 06:30:15', 1, '/Sidebar/ReviewFeedback'),
(8, 1, 'Your maintenance request for room 301 has been submitted successfully', 'service', 'fas fa-tools', 5, '2025-05-07 06:32:46', 1, '/Sidebar/ServiceRequest'),
(9, 1, 'Your housekeeping request for room 301 has been submitted successfully', 'service', 'fas fa-broom', 6, '2025-05-07 06:32:53', 1, '/Sidebar/ServiceRequest'),
(10, 1, 'Your maintenance request for Room 301 (Plumbing) has been marked as In Progress. Our maintenance staff are on their way to address the issue.', 'Maintenance', 'fas fa-tools', 5, '2025-05-07 06:34:47', 1, '/Service/RequestStatus?id=5'),
(11, 1, 'Your maintenance request for Room 301 (Plumbing) has been completed. Thank you for your patience.', 'Maintenance', 'fas fa-tools', 5, '2025-05-07 06:34:49', 1, '/Service/RequestStatus?id=5'),
(12, 1, 'Your housekeeping request for Room 301 (Room Cleaning) has been started. Our housekeeping staff will attend to your room shortly.', 'Housekeeping', 'fas fa-broom', 6, '2025-05-07 06:37:12', 1, '/Service/RequestStatus?id=6'),
(13, 1, 'Your housekeeping request for Room 301 (Room Cleaning) has been completed. We hope everything is to your satisfaction.', 'Housekeeping', 'fas fa-broom', 6, '2025-05-07 06:37:14', 1, '/Service/RequestStatus?id=6');

-- --------------------------------------------------------

--
-- Table structure for table `room`
--

CREATE TABLE `room` (
  `ROOM_ID` int(11) NOT NULL,
  `ROOM_NUMBER` varchar(10) NOT NULL,
  `ROOM_TYPE` varchar(50) NOT NULL,
  `STATUS` varchar(50) NOT NULL,
  `STATUS_UPDATED_AT` datetime NOT NULL,
  `UPDATED_BY` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `room`
--

INSERT INTO `room` (`ROOM_ID`, `ROOM_NUMBER`, `ROOM_TYPE`, `STATUS`, `STATUS_UPDATED_AT`, `UPDATED_BY`) VALUES
(1, '301', 'standard', 'maintenance', '2025-05-06 21:57:55', 'Jamie Roberts'),
(2, '302', 'standard', 'available', '2025-05-04 09:39:18', 'Maverick Villarta'),
(3, '303', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(4, '304', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(5, '305', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(6, '306', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(7, '307', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(8, '308', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(9, '309', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(10, '310', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(11, '311', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(12, '312', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(13, '313', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(14, '314', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(15, '315', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(16, '316', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(17, '317', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(18, '318', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(19, '319', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(20, '320', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(21, '321', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(22, '322', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(23, '323', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(24, '324', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(25, '325', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(26, '326', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(27, '327', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(28, '328', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(29, '329', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(30, '330', 'standard', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(31, '401', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(32, '402', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(33, '403', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(34, '404', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(35, '405', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(36, '406', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(37, '407', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(38, '408', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(39, '409', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(40, '410', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(41, '411', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(42, '412', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(43, '413', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(44, '414', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(45, '415', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(46, '416', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(47, '417', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(48, '418', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(49, '419', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(50, '420', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(51, '421', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(52, '422', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(53, '423', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(54, '424', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(55, '425', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(56, '426', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(57, '427', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(58, '428', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(59, '429', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(60, '430', 'deluxe', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(61, '501', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(62, '502', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(63, '503', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(64, '504', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(65, '505', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(66, '506', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(67, '507', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(68, '508', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(69, '509', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(70, '510', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(71, '511', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(72, '512', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(73, '513', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(74, '514', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(75, '515', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(76, '516', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(77, '517', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(78, '518', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(79, '519', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(80, '520', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(81, '521', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(82, '522', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(83, '523', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(84, '524', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(85, '525', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(86, '526', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(87, '527', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(88, '528', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(89, '529', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)'),
(90, '530', 'suite', 'available', '2025-05-04 09:38:50', 'Maverick Villarta (Global Reset)');

-- --------------------------------------------------------

--
-- Table structure for table `service_request`
--

CREATE TABLE `service_request` (
  `REQUEST_ID` int(11) NOT NULL,
  `USER_ID` int(11) NOT NULL,
  `ROOM_NUMBER` varchar(10) NOT NULL,
  `REQUEST_TYPE` varchar(20) NOT NULL,
  `SERVICE_CATEGORY` varchar(50) DEFAULT NULL,
  `HOUSEKEEPING_NEEDS` varchar(255) DEFAULT NULL,
  `DESCRIPTION` text NOT NULL,
  `REQUEST_DATE` datetime NOT NULL,
  `STATUS` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `service_request`
--

INSERT INTO `service_request` (`REQUEST_ID`, `USER_ID`, `ROOM_NUMBER`, `REQUEST_TYPE`, `SERVICE_CATEGORY`, `HOUSEKEEPING_NEEDS`, `DESCRIPTION`, `REQUEST_DATE`, `STATUS`) VALUES
(1, 1, '405', 'maintenance', 'Electrical', NULL, 'guba inyong aircon', '2025-05-04 19:04:31', 'Completed'),
(2, 1, '405', 'housekeeping', '', 'Toiletries', 'way sabon', '2025-05-04 19:04:52', 'Completed'),
(3, 1, '301', 'maintenance', 'Electrical', NULL, 'aircon', '2025-05-07 05:54:44', 'Completed'),
(4, 1, '301', 'housekeeping', '', 'Toiletries', 'way agas', '2025-05-07 05:54:53', 'Completed'),
(5, 1, '301', 'maintenance', 'Plumbing', NULL, 'la', '2025-05-07 06:32:45', 'Completed'),
(6, 1, '301', 'housekeeping', '', 'Fresh towels', 'la', '2025-05-07 06:32:53', 'Completed');

-- --------------------------------------------------------

--
-- Table structure for table `settings`
--

CREATE TABLE `settings` (
  `SETTING_ID` int(11) NOT NULL,
  `SETTING_KEY` varchar(50) NOT NULL,
  `SETTING_VALUE` varchar(500) NOT NULL,
  `DESCRIPTION` varchar(100) DEFAULT NULL,
  `LAST_UPDATED` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `settings`
--

INSERT INTO `settings` (`SETTING_ID`, `SETTING_KEY`, `SETTING_VALUE`, `DESCRIPTION`, `LAST_UPDATED`) VALUES
(1, 'EventPageTitle', 'April Events', 'Title displayed on the Events page', '2025-05-06 16:45:33');

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `USER_ID` int(11) NOT NULL,
  `FIRST_NAME` varchar(50) NOT NULL,
  `LAST_NAME` varchar(50) NOT NULL,
  `CONTACT` longtext NOT NULL,
  `EMAIL` varchar(255) NOT NULL,
  `PASSWORD` varchar(100) NOT NULL,
  `ROLE` longtext NOT NULL,
  `PROFILE_PICTURE_PATH` longtext DEFAULT NULL,
  `MEMBER_SINCE` datetime(6) NOT NULL,
  `DEPARTMENT` longtext DEFAULT NULL,
  `HIRE_DATE` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`USER_ID`, `FIRST_NAME`, `LAST_NAME`, `CONTACT`, `EMAIL`, `PASSWORD`, `ROLE`, `PROFILE_PICTURE_PATH`, `MEMBER_SINCE`, `DEPARTMENT`, `HIRE_DATE`) VALUES
(1, 'Bonch', 'Eturma', '09123456789', 'bonch.eturma@example.com', 'pass123', 'Guest', '/images/profiles/bcc20e11-fafc-431c-9de2-679a65d32932.png', '2025-04-15 22:55:29.000000', NULL, NULL),
(2, 'Admin', 'User', '09123456788', 'admin@hrsm.com', 'adminpass123', 'Admin', NULL, '2025-04-15 22:55:29.000000', NULL, NULL),
(6, 'Maverick', 'Villarta', '09872312312', 'maverick.villarta@example.com', 'pass12', 'Staff', '/images/profiles/0adb8ced-3302-4fe8-8cad-83c04164670f.png', '2025-04-16 16:06:34.212339', 'Receptionist', '2025-04-18 00:00:00.000000'),
(8, 'Ram', 'Alin', '09823123123', 'ram.alin@example.com', 'atay12', 'Staff', NULL, '2025-04-16 16:46:22.489285', 'Receptionist', '2025-04-18 00:00:00.000000'),
(9, 'Vanness', 'Benitez', '09823481312', 'van.benitez@example.com', 'van123', 'Staff', NULL, '2025-04-16 16:48:33.364392', 'Housekeeping', '2025-04-18 00:00:00.000000'),
(10, 'Felipe', 'Aquino', '09365556780', 'felipe.aquino@example.com', 'pass123', 'Staff', NULL, '2025-04-16 17:05:00.713420', 'Housekeeping', '2025-04-18 00:00:00.000000'),
(11, 'Jamie', 'Roberts', '09213288121', 'jamie.roberts@example.com', 'atay12', 'Staff', NULL, '2025-04-16 17:06:41.083165', 'Maintenance', '2025-04-18 00:00:00.000000'),
(12, 'Lucian', 'Crowhart', '09832712312', 'lucian@example.com', 'piste123', 'Staff', NULL, '2025-04-16 17:07:32.928109', 'Maintenance', '2025-04-18 00:00:00.000000'),
(13, 'Andres', 'Bonifacio', '09231823123', 'andres@example.com', 'pass123123', 'Guest', '/images/default-profile.png', '2025-04-23 02:07:21.172222', NULL, NULL),
(14, 'Jose', 'Rizal', '09823812312', 'jose.rizal@example.com', 'jose123', 'Guest', '/images/default-profile.png', '2025-05-07 06:47:21.448345', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250415145059_InitialCreate', '8.0.13');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `activity`
--
ALTER TABLE `activity`
  ADD PRIMARY KEY (`ACTIVITY_ID`);

--
-- Indexes for table `booking`
--
ALTER TABLE `booking`
  ADD PRIMARY KEY (`BOOKING_ID`),
  ADD KEY `IX_BOOKING_USER_ID` (`USER_ID`);

--
-- Indexes for table `event`
--
ALTER TABLE `event`
  ADD PRIMARY KEY (`EVENT_ID`);

--
-- Indexes for table `feedback`
--
ALTER TABLE `feedback`
  ADD PRIMARY KEY (`FEEDBACK_ID`),
  ADD KEY `USER_ID` (`USER_ID`);

--
-- Indexes for table `notification`
--
ALTER TABLE `notification`
  ADD PRIMARY KEY (`NOTIFICATION_ID`),
  ADD KEY `USER_ID` (`USER_ID`);

--
-- Indexes for table `room`
--
ALTER TABLE `room`
  ADD PRIMARY KEY (`ROOM_ID`),
  ADD UNIQUE KEY `ROOM_NUMBER` (`ROOM_NUMBER`);

--
-- Indexes for table `service_request`
--
ALTER TABLE `service_request`
  ADD PRIMARY KEY (`REQUEST_ID`),
  ADD KEY `USER_ID` (`USER_ID`);

--
-- Indexes for table `settings`
--
ALTER TABLE `settings`
  ADD PRIMARY KEY (`SETTING_ID`),
  ADD UNIQUE KEY `SETTING_KEY` (`SETTING_KEY`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`USER_ID`),
  ADD UNIQUE KEY `IX_USER_EMAIL` (`EMAIL`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `activity`
--
ALTER TABLE `activity`
  MODIFY `ACTIVITY_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=102;

--
-- AUTO_INCREMENT for table `booking`
--
ALTER TABLE `booking`
  MODIFY `BOOKING_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=48;

--
-- AUTO_INCREMENT for table `event`
--
ALTER TABLE `event`
  MODIFY `EVENT_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `feedback`
--
ALTER TABLE `feedback`
  MODIFY `FEEDBACK_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `notification`
--
ALTER TABLE `notification`
  MODIFY `NOTIFICATION_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `room`
--
ALTER TABLE `room`
  MODIFY `ROOM_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=116;

--
-- AUTO_INCREMENT for table `service_request`
--
ALTER TABLE `service_request`
  MODIFY `REQUEST_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `settings`
--
ALTER TABLE `settings`
  MODIFY `SETTING_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `USER_ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `booking`
--
ALTER TABLE `booking`
  ADD CONSTRAINT `FK_BOOKING_USER_USER_ID` FOREIGN KEY (`USER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE;

--
-- Constraints for table `feedback`
--
ALTER TABLE `feedback`
  ADD CONSTRAINT `feedback_ibfk_1` FOREIGN KEY (`USER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE;

--
-- Constraints for table `notification`
--
ALTER TABLE `notification`
  ADD CONSTRAINT `notification_ibfk_1` FOREIGN KEY (`USER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE;

--
-- Constraints for table `service_request`
--
ALTER TABLE `service_request`
  ADD CONSTRAINT `service_request_ibfk_1` FOREIGN KEY (`USER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
