package com.example.inspectxr.controller;

import com.example.inspectxr.dtos.SessionRequestDTO;
import com.example.inspectxr.dtos.SessionResponseDTO;
import com.example.inspectxr.service.SessionService;

import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.*;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@CrossOrigin(origins = "http://localhost:8081")
@RestController
@RequestMapping("/sessions")
public class SessionController {

    private final SessionService sessionService;

    @Autowired
    public SessionController(SessionService sessionService) {
        this.sessionService = sessionService;
    }

    @PostMapping
    public ResponseEntity<SessionResponseDTO> createSession(@Valid @RequestBody SessionRequestDTO dto) {
        return new ResponseEntity<>(sessionService.createSession(dto), HttpStatus.CREATED);
    }

    @GetMapping("/{id}")
    public ResponseEntity<SessionResponseDTO> getSession(@PathVariable Long id) {
        return new ResponseEntity<>(sessionService.getSession(id), HttpStatus.OK);
    }

    @GetMapping
    public ResponseEntity<List<SessionResponseDTO>> getAllSessions() {
        return new ResponseEntity<>(sessionService.getAllSessions(), HttpStatus.OK);
    }

    @GetMapping("/owner/{ownerId}")
    public ResponseEntity<?> getSessionsByOwnerId(@PathVariable Long ownerId) {
        try {
            return new ResponseEntity<>(sessionService.getSessionsByOwnerId(ownerId), HttpStatus.OK);
        } catch (Exception e) {
            return new ResponseEntity<>("Error: " + e.getMessage(), HttpStatus.NOT_FOUND);
        }
    }
}
